using System;
using System.Collections;
using System.Collections.Generic;
using TUIOsharp.DataProcessors;
using UnityEngine;
using Object = System.Object;


public enum Objects
{
    Mirror,
    Wall
}

public class ObjectManager : MonoBehaviour
{
    private Dictionary<int, long> _gameObjects;

    void Awake()
    {
        _gameObjects = new Dictionary<int, long>();
        TUIOInput.OnObjectAdded += OnObjectAdded;
    }

    private void OnObjectAdded(object sender, TuioObjectEventArgs e)
    {
        if (_gameObjects.ContainsKey(e.Object.Id))
        {
            Debug.LogError(("An object input was received for an object that already exists"));
            return;
        }

        if (!Enum.IsDefined(typeof(Objects), e.Object.ClassId))
        {
            Debug.LogError($"Unknown Object class id: {e.Object.ClassId}");
            return;
        }

        var type = (Objects)e.Object.ClassId;

        _gameObjects.Add(e.Object.Id, GameManager.SpawnPrefab(type.ToString(), new Vector3((e.Object.X - .5f) * 44f, 0, (e.Object.Y - .5f) * 44f), Vector3.forward));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
