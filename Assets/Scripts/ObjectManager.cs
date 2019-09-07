using System;
using System.Collections.Generic;
using TUIOsharp.DataProcessors;
using UnityEngine;

public enum Objects
{
    Mirror,
    Wall
}

public class ObjectManager : MonoBehaviour
{
    private Dictionary<int, long> _gameObjects;

    private void Awake()
    {
        _gameObjects = new Dictionary<int, long>();
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
        var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * e.Object.Angle, Vector3.up);
        var gameId = GameManager.SpawnPrefab(type.ToString(), ScreenToWorld(e.Object.X, e.Object.Y), rot);
        _gameObjects.Add(e.Object.Id, gameId);
    }

    private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
    {
        if (!_gameObjects.ContainsKey(e.Object.Id))
        {
            //TODO: Perhaps we should just add it instead?
            Debug.LogError("Tried to update an object that was not added priorly.");
            return;
        }

        Debug.Log(e.Object.Angle);
        var gameId = _gameObjects[e.Object.Id];
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

        GameManager.RemoveSpawnedObject(_gameObjects[e.Object.Id]);
    }

    private Vector3 ScreenToWorld(float x, float y)
    {
        return new Vector3((x - .5f) * 44f, 0, (.5f - y) * 44f);
    }
}