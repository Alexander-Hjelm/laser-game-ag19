using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Spawnable objects
public enum ObjectType
{
    Mirror,
    Prism,
    Laser,
}

public class ObjectManager : MonoBehaviour
{
    // Availability of each Object Type and its corresponding class ids
    public (ObjectType, int, int)[] ClassAvailability = new (ObjectType, int, int)[]
    {
        (ObjectType.Mirror, 0, 1),
        (ObjectType.Mirror, 2, 3),
        (ObjectType.Mirror, 4, 5),
        (ObjectType.Mirror, 6, 7),
        (ObjectType.Mirror, 8, 9),
        (ObjectType.Prism, 10, 11),
        (ObjectType.Prism, 12, 13),
        (ObjectType.Prism, 14, 15),
        (ObjectType.Prism, 16, 17),
        (ObjectType.Laser, 18, 19),
        (ObjectType.Laser, 20, 21),
        (ObjectType.Laser, 22, 23),
        (ObjectType.Laser, 24, 25),
    };
    [System.Serializable]
    public class MaxObject { public ObjectType type; public int max; }

    [SerializeField()]
    public MaxObject[] MaxObjectsPerType; // List containing limits for each object type
    [SerializeField()]
    public GameObject AuraDisplay;

    // Aura shader values
    private Vector4[] _shaderArray = new Vector4[10];
    private float[] _shaderAngleArray = new float[10];

    // All spawned game objects from screen objects go here
    // (unique id from classId1 + classId2, GameObject Id, Type)
    private Dictionary<int, (long, ObjectType)> _gameObjects = new Dictionary<int, (long, ObjectType)>();

    private Zone[] zones;
    private void Awake()
    {
        zones = FindObjectsOfType<Zone>();
    }

    private void Update()
    {
        // A proper mod function that turns negative values into positive
        float mod(float x, float m)
        {
            return (x % m + m) % m;
        }

        List<Vector4> auraObjects = new List<Vector4>();
        List<float> auraAngles = new List<float>();

        foreach (var (type, classId1, classId2) in ClassAvailability)
        {
            var uniqueId = classId1 + classId2;
            // First check if there are two of these in screen objects
            var screenObject1 = TUIOInput.GetScreenObjects(classId1).FirstOrDefault();
            var screenObject2 = TUIOInput.GetScreenObjects(classId2).FirstOrDefault();
            if (screenObject1 != null && screenObject2 != null)
            {
                // If we have these two, we either spawn or update a gameobject
                if (_gameObjects.ContainsKey(uniqueId))
                {
                    // We have already spawned this game object, instead update it
                    var (gameId,_) = _gameObjects[uniqueId];

                    // Calculate angle for next tick
                    var angle = Direction(screenObject1, screenObject2);
                    var targetAngle = Mathf.Rad2Deg * angle;
                    var transform = GameManager.GetSpawnedObject(gameId).transform;
                    var currAngle = transform.eulerAngles.y;
                    // Two ways of turning towards that angle
                    var deltaAngle = mod(targetAngle - currAngle, 360);
                    var negativeDeltaAngle = deltaAngle - 360f;
                    // Choose the angle that needs the least amount of rotation
                    deltaAngle = -negativeDeltaAngle < deltaAngle ? negativeDeltaAngle : deltaAngle;
                    // Multiply by constant rotational speed
                    deltaAngle *= 2f;
                    // Add to our current angle
                    currAngle += deltaAngle * Time.deltaTime;
                    var rot = Quaternion.AngleAxis(currAngle, Vector3.up);

                    var pos = ScreenToWorld(Position(screenObject1, screenObject2));
                    GameManager.SetPositionOfSpawnedObject(gameId, pos);
                    GameManager.SetRotationOfSpawnedObject(gameId, rot);
                    // If we get removed from a zone simply remove ourselves
                    if (!UpdateObjectInZone(gameId, type))
                    {
                        // Object not in zone, remove this object
                        GameManager.RemoveSpawnedObject(_gameObjects[uniqueId].Item1);
                        _gameObjects.Remove(uniqueId);
                    }
                }
                else
                {
                    var pos = ScreenToWorld(Position(screenObject1, screenObject2));
                    // We need to spawn it
                    var (canSpawn, zone) = CanSpawn(type, pos);
                    if (canSpawn)
                    {
                        // if we can be spawned, spawn us within that zone
                        var angle = Direction(screenObject1, screenObject2);
                        var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.up);
                        var gameId = GameManager.SpawnPrefab(type.ToString(), pos, rot);
                        _gameObjects.Add(uniqueId, (gameId, type));
                        zone?.Place(gameId);
                    }
                    else
                    {
                        // Cannot spawn it, e.g. faulty usage of phycon, thus display an aura around it
                        var screenPos = Position(screenObject1, screenObject2);
                        auraObjects.Add(new Vector4(screenPos.x, screenPos.y, 0.1f, 0.1f));
                        var angle = Direction(screenObject1, screenObject2);
                        auraAngles.Add(angle);
                    }
                }
            }
            else
            {
                // Otherwise we must remove any occurence of this object
                if (_gameObjects.ContainsKey(uniqueId))
                {
                    GameManager.RemoveSpawnedObject(_gameObjects[uniqueId].Item1);
                    _gameObjects.Remove(uniqueId);
                }
            }
        }

        // Finally update the aura
        var rw = AuraDisplay.GetComponent<RawImage>();
        rw.material.SetInt("_ObjectsLength", auraObjects.Count);
        if (auraObjects.Count > 0)
        {
            auraObjects.CopyTo(_shaderArray);
            auraAngles.CopyTo(_shaderAngleArray);
            rw.material.SetVectorArray("_Objects", _shaderArray);
            rw.material.SetFloatArray("_ObjectAngles", _shaderAngleArray);
        }
    }

    private Vector3 ScreenToWorld(Vector2 pos)
    {
        return ScreenToWorld(pos.x, pos.y);
    }

    private Vector3 ScreenToWorld(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(x,  y, Camera.main.transform.position.y)); // y = 0 is our playing field plane
    }

    private Vector2 Position(ScreenObject obj1, ScreenObject obj2)
    {
        return (obj1.screenPosition + obj2.screenPosition) * .5f;
    }

    /**
     * Returns the angle of the direction from pos obj1 to pos obj2
     */
    private float Direction(ScreenObject obj1, ScreenObject obj2)
    {
        var delta = obj2.screenPosition - obj1.screenPosition;
        return Mathf.Atan2(delta.x, delta.y);
    }

    private (bool, Zone) CanSpawn(ObjectType type, Vector3 pos)
    {
        var m = MaxObjectsPerType.FirstOrDefault(mopt => mopt.type == type);

        // No max amount => any amount
        var zonesOfType = zones.Where(zone => zone.Type == type);
        bool canSpawnZone = !zonesOfType.Any();

        Zone retZone = null;
        foreach (var zone in zonesOfType)
        {
            if (!zone.IsInside(pos)) continue;

            canSpawnZone = true;
            retZone = zone;
        }

        if (m == null)
        {
            return (false, null);
        }

        return CountSpawnedObjectsOfType(type) < m.max ? (canSpawnZone, retZone) : (false, null);
    }

    private bool UpdateObjectInZone(long id, ObjectType type)
    {
        var objectZone = zones.Where(zone => zone.Type == type && zone.Contains(id));
        foreach (var zone in objectZone)
        {
            return zone.UpdateGameObject(id);
        }

        return true;
    }

    public MaxObject[] GetMaxObjectsPerType () {
        return MaxObjectsPerType;
    }

    public int CountSpawnedObjectsOfType(ObjectType type) {
        return _gameObjects.Count((obj) => obj.Value.Item2 == type);
    }

    public int CountSpawnedObjects()
    {
        return _gameObjects.Count;
    }
}
