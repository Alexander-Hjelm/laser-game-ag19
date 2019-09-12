using System;
using System.Collections.Generic;
using System.Linq;
using TUIOsharp.DataProcessors;
using UnityEngine;

public enum Objects
{
    Mirror,
    Wall
}

public class ObjectManager : MonoBehaviour
{
    [System.Serializable]
    public class MaxObject { public Objects type; public int max; }

    [SerializeField()]
    public MaxObject[] MaxObjectsPerType;

    private struct TUIOObject
    {
        public int? id;
        public long gameId;
        public Objects type;
    }

    private Dictionary<int, TUIOObject> _gameObjects;
    private void Awake()
    {
        _gameObjects = new Dictionary<int, TUIOObject>();
        TUIOInput.OnObjectAdded += OnObjectAdded;
        TUIOInput.OnObjectUpdated += OnObjectUpdated;
        TUIOInput.OnObjectRemoved += OnObjectRemoved;
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
        var maxStruct = MaxObjectsPerType.FirstOrDefault(m => m.type == type);

        if (maxStruct != null && _gameObjects.Values.Count(g => g.type == type) >= maxStruct.max)
        {
            Debug.Log("More than one");
            return;
        }

        var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * e.Object.Angle, Vector3.up);
        var gameId = GameManager.SpawnPrefab(type.ToString(), ScreenToWorld(e.Object.X, e.Object.Y), rot);
        var tuioObj = new TUIOObject()
        {
            id = e.Object.Id,
            gameId = gameId,
            type = type
        };
        _gameObjects.Add(e.Object.Id, tuioObj);
    }

    private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
    {
        if (!_gameObjects.ContainsKey(e.Object.Id))
        {
            Debug.LogWarning("Tried to update an object that was not added before. Maybe you reached max objects?");
            return;
        }

        var gameId = _gameObjects[e.Object.Id].gameId;
        var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * e.Object.Angle, Vector3.up);
        GameManager.SetPositionOfSpawnedObject(gameId, ScreenToWorld(e.Object.X, e.Object.Y));
        GameManager.SetRotationOfSpawnedObject(gameId, rot);
    }

    private void OnObjectRemoved(object sender, TuioObjectEventArgs e)
    {
        if (!_gameObjects.ContainsKey(e.Object.Id))
        {
            Debug.LogError("Tried to remove an object that was not added priorly.");
            return;
        }

        GameManager.RemoveSpawnedObject(_gameObjects[e.Object.Id].gameId);
        _gameObjects.Remove(e.Object.Id);
    }

    private Vector3 ScreenToWorld(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(x, 1 - y, Camera.main.transform.position.y)); // y = 0 is our playing field plane
    }
}