using System;
using System.Collections.Generic;
using System.Linq;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using UnityEngine;

public class ScreenObject
{
    public int id;
    public int classId;
    public Vector2 screenPosition;
    public float angle;
}

public class TUIOInput : MonoBehaviour
{
    private static TUIOInput _instance; //singleton

    private readonly int _port = 3333;

    private TuioServer tuioServer;
    private Dictionary<int, ScreenObject> _screenObjects;

    private void Awake()
    {
        if (_instance != null) throw new UnityException("Two instances of a TUIOInput was found. Please remove one.");
        _screenObjects = new Dictionary<int, ScreenObject>();
        ListenForTUIO();
        _instance = this;
        Debug.Log("Started TUIO Client");
    }

    private void OnDestroy()
    {
        tuioServer.Disconnect();
        tuioServer = null;
        _instance = null;
        _screenObjects = null;
    }

    private void ListenForTUIO()
    {
        // tuio
        tuioServer = new TuioServer(_port);
        tuioServer.Connect();

        Debug.Log(string.Format("TUIO listening on port {0}.", _port));

        // Add Object Processor
        var objectProcessor = new ObjectProcessor();
        objectProcessor.ObjectAdded += (sender, e) =>
        {
            if (_screenObjects.ContainsKey(e.Object.Id))
            {
                Debug.LogError("A screen object input was received for an object that already exists.");
                return;
            }

            var obj = new ScreenObject()
            {
                id = e.Object.Id,
                classId = e.Object.ClassId,
                screenPosition = new Vector2(e.Object.X, 1 - e.Object.Y),
                angle = e.Object.Angle
            };

            _screenObjects.Add(e.Object.Id, obj);
        };

        objectProcessor.ObjectUpdated += (sender, e) =>
        {
            if (!_screenObjects.ContainsKey(e.Object.Id))
            {
                Debug.LogError("Tried to update a screen object that was not added priorly.");
                return;
            }

            var go = _screenObjects[e.Object.Id];
            go.screenPosition = new Vector2(e.Object.X, 1 - e.Object.Y);
            go.angle = e.Object.Angle;
        };
        ;
        objectProcessor.ObjectRemoved += (sender, e) =>
        {
            if (!_screenObjects.ContainsKey(e.Object.Id))
            {
                Debug.LogError("Tried to remove a screen object that was not added priorly.");
                return;
            }

            _screenObjects.Remove(e.Object.Id);
        };
        tuioServer.AddDataProcessor(objectProcessor);
    }

    public static IEnumerable<ScreenObject> GetScreenObjects()
    {
        return _instance._screenObjects.Values;
    }

    public static IEnumerable<ScreenObject> GetScreenObjects(int classId)
    {
        return _instance._screenObjects.Values.Where(obj => obj.classId == classId);
    }
}