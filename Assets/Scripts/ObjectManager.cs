using System;
using System.Collections.Generic;
using System.Linq;
using TUIOsharp.DataProcessors;
using UnityEngine;
using UnityEngine.UI;

// Spawnable objects
public enum Objects
{
    Mirror,
    Prism,
    Laser
}

public class ObjectManager : MonoBehaviour
{
    
    [System.Serializable]
    public class ObjectScreenSize { public Objects type; public Vector2 size; }
    [System.Serializable]
    public class MaxObject { public Objects type; public int max; }

    [SerializeField()]
    public MaxObject[] MaxObjectsPerType; // List containing limits for each object type
    [SerializeField()]
    public ObjectScreenSize[] ObjectScreenSizes;
    public GameObject AuraDisplay;

    private class TUIOObject
    {
        public int id;
        public long gameId;
        public Objects type;
        public Vector2 screenPosition;
        public float angle;
        public Zone zone;
    }

    private Dictionary<int, TUIOObject> _gameObjects;
    private Dictionary<int, TUIOObject> _badObjects;

    private Vector4[] _shaderArray;
    private float[] _shaderAngleArray;

    private Zone[] zones;

    private void Awake()
    {
        _shaderArray = new Vector4[10];
        _shaderAngleArray = new float[10];
        _gameObjects = new Dictionary<int, TUIOObject>();
        _badObjects = new Dictionary<int, TUIOObject>();
        TUIOInput.OnObjectAdded += OnObjectAdded;
        TUIOInput.OnObjectUpdated += OnObjectUpdated;
        TUIOInput.OnObjectRemoved += OnObjectRemoved;
        zones = FindObjectsOfType<Zone>();
    }

    private void Update()
    {
        var rw = AuraDisplay.GetComponent<RawImage>();

        var arr = _badObjects.Values.Select(go =>
        {
            var oss = ObjectScreenSizes.FirstOrDefault(o => o.type == go.type);
            if (oss == null)
            {
                return new Vector4(go.screenPosition.x, go.screenPosition.y, 0.1f, 0.1f);
            }
            return new Vector4(go.screenPosition.x, go.screenPosition.y, oss.size.x, oss.size.y);
        }).ToArray();
        Array.Copy(arr, _shaderArray, arr.Length);
        var angles = _badObjects.Values.Select(go => go.angle).ToArray();
        Array.Copy(angles, _shaderAngleArray, angles.Length);
        rw.material.SetInt("_ObjectsLength", arr.Length);
        if (arr.Length > 0)
        {
            rw.material.SetVectorArray("_Objects", _shaderArray);
            rw.material.SetFloatArray("_ObjectAngles",_shaderAngleArray);
        }
    }

    private void OnObjectAdded(object sender, TuioObjectEventArgs e)
    {
        if (_gameObjects.ContainsKey(e.Object.Id))
        {
            Debug.LogError("An object input was received for an object that already exists");
            return;
        }

        if (!Enum.IsDefined(typeof(Objects), e.Object.ClassId))
        {
            Debug.LogError($"Unknown Object class id: {e.Object.ClassId}");
            return;
        }

        var type = (Objects) e.Object.ClassId;

        var tuioObj = new TUIOObject()
        {
            id = e.Object.Id,
            type = type,
            screenPosition = new Vector2(e.Object.X, 1 - e.Object.Y),
            angle = e.Object.Angle
        };

        if (MaxObjectsReached(tuioObj.type))
        {
            _badObjects.Add(tuioObj.id, tuioObj);
            return;
        }

        var (isPlaced, zone) = TryPlace(tuioObj);
        if (isPlaced)
        {
            AddObjectToWorld(tuioObj, zone);
            tuioObj.zone = zone;
            zone.SetGraphicsEnabled(false);
        }
        else
        {
            _badObjects.Add(tuioObj.id, tuioObj);
            zone.SetGraphicsEnabled(true);
        }
    }

    private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
    {
        if (!_gameObjects.ContainsKey(e.Object.Id))
        {
            if (_badObjects.ContainsKey(e.Object.Id))
            {
                var badgo = _badObjects[e.Object.Id];
                badgo.screenPosition = new Vector2(e.Object.X, 1 - e.Object.Y);
                badgo.angle = e.Object.Angle;
                // Let's see if the bad object is now a proper one
                // First check if max objects has been reached
                if (!MaxObjectsReached(badgo.type))
                {
                    // Try to place the object
                    var (isPlaced, zone) = TryPlace(badgo);
                    if (isPlaced)
                    {
                        // If it was placed add it to the world and set the correct zone
                        AddObjectToWorld(badgo, zone);
                        _badObjects.Remove(e.Object.Id);
                        badgo.zone = zone;
                        zone.SetGraphicsEnabled(false);
                    }
                }
                return;
            }
            Debug.LogWarning("Tried to update an object that was not added before. Maybe you reached max objects?");
            return;
        }
        var go = _gameObjects[e.Object.Id];
        var gameId = go.gameId;
        go.screenPosition = new Vector2(e.Object.X, 1 - e.Object.Y);
        go.angle = e.Object.Angle;
        var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * go.angle, Vector3.up);
        GameManager.SetPositionOfSpawnedObject(gameId, ScreenToWorld(go.screenPosition));
        GameManager.SetRotationOfSpawnedObject(gameId, rot);

        var (placed, z) = TryPlace(go);
        // If we are no longer within a correct zone, or the same zone turn the object into a bad object
        if (!placed || z != go.zone)
        {
            GameManager.RemoveSpawnedObject(gameId);
            _gameObjects.Remove(go.id);
            _badObjects.Add(go.id, go);
            go.gameId = 0;
            go.zone.SetGraphicsEnabled(true);
            go.zone = null;
        }
    }

    private void OnObjectRemoved(object sender, TuioObjectEventArgs e)
    {
        if (!_gameObjects.ContainsKey(e.Object.Id))
        {
            if (_badObjects.ContainsKey(e.Object.Id))
            {
                _badObjects.Remove(e.Object.Id);
                return;
            }
            Debug.LogError("Tried to remove an object that was not added priorly.");
            return;
        }

        GameManager.RemoveSpawnedObject(_gameObjects[e.Object.Id].gameId);
        _gameObjects.Remove(e.Object.Id);
    }

    private Vector3 ScreenToWorld(Vector2 pos)
    {
        return ScreenToWorld(pos.x, pos.y);
    }

    private Vector3 ScreenToWorld(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(x,  y, Camera.main.transform.position.y)); // y = 0 is our playing field plane
    }

    private void AddObjectToWorld(TUIOObject obj, Zone zone)
    {
        var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * obj.angle, Vector3.up);
        var gameId = GameManager.SpawnPrefab(obj.type.ToString(), ScreenToWorld(obj.screenPosition), rot);
        obj.gameId = gameId;
        _gameObjects.Add(obj.id, obj);
        zone.OnEnter(GameManager.GetSpawnedObject(obj.gameId));

    }
    
    private (bool, Zone) TryPlace(TUIOObject obj)
    {
        var worldPos = ScreenToWorld(obj.screenPosition);
        var zonesOfType = zones.Where(zone => zone.Type == obj.type);

        if (!zonesOfType.Any())
        {
            // If there is no zone of this type, allow placement allover
            return (true, null);
        }

        var inside = false;
        Zone retZone = null;
        foreach (var zone in zones)
        {
            if (zone.Type != obj.type) continue;

            // IsPlaceable needs to be called on every single zone, as it acts as an update as well
            // However, if we can be placed in one zone, it means object can be placed
            if (!zone.TryPlace(obj.id, worldPos)) continue;

            inside = true;
            retZone = zone;
        }

        return (inside, retZone);
    }

    private bool MaxObjectsReached(Objects type)
    {
        var maxStruct = MaxObjectsPerType.FirstOrDefault(m => m.type == type);
        return maxStruct == null || _gameObjects.Values.Count(g => g.type == type) >= maxStruct.max;
    }

    public MaxObject[] GetMaxObjectsPerType () {
        return MaxObjectsPerType;
    }

    public int CountSpawnedObjectsOfType(Objects type) {
        return _gameObjects.Values.Count(g => g.type == type);
    }
}
