using System;
using System.Collections.Generic;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using UnityEngine;

public class TUIOInput : MonoBehaviour
{
    private static TUIOInput _instance; //singleton

    private readonly int _port = 3333;

    // We want to stagger all events to make them run on the main unity thread
    private List<(object, TuioObjectEventArgs)> _staggeredOnObjectAdded;
    private List<(object, TuioObjectEventArgs)> _staggeredOnObjectRemoved;
    private List<(object, TuioObjectEventArgs)> _staggeredOnObjectUpdated;
    private TuioServer tuioServer;

    public static event EventHandler<TuioObjectEventArgs> OnObjectAdded;
    public static event EventHandler<TuioObjectEventArgs> OnObjectRemoved;
    public static event EventHandler<TuioObjectEventArgs> OnObjectUpdated;

    private void Awake()
    {
        if (_instance != null) throw new UnityException("Two instances of a TUIOInput was found. Please remove one."); 
        _staggeredOnObjectAdded = new List<(object, TuioObjectEventArgs)>();
        _staggeredOnObjectRemoved = new List<(object, TuioObjectEventArgs)>();
        _staggeredOnObjectUpdated = new List<(object, TuioObjectEventArgs)>();
        ListenForTUIO();
        _instance = this;
        Debug.Log("Started TUIO Client");
    }

    private void OnDestroy()
    {
        tuioServer.Disconnect();
        tuioServer = null;
        _instance = null;
        OnObjectAdded = null;
        OnObjectRemoved = null;
        OnObjectUpdated = null;
    }

    private void Update()
    {
        _staggeredOnObjectAdded.RemoveAll(args =>
        {
            OnObjectAdded?.Invoke(args.Item1, args.Item2);
            return true;
        });
        _staggeredOnObjectRemoved.RemoveAll(args =>
        {
            OnObjectRemoved?.Invoke(args.Item1, args.Item2);
            return true;
        });
        _staggeredOnObjectUpdated.RemoveAll(args =>
        {
            OnObjectUpdated?.Invoke(args.Item1, args.Item2);
            return true;
        });
    }

    private void ListenForTUIO()
    {
        // tuio
        tuioServer = new TuioServer(_port);
        tuioServer.Connect();

        Debug.Log(string.Format("TUIO listening on port {0}.", _port));

        // Add Object Processor
        var objectProcessor = new ObjectProcessor();
        objectProcessor.ObjectAdded += (sender, e) => { _staggeredOnObjectAdded.Add((sender, e)); };
        objectProcessor.ObjectUpdated += (sender, e) => { _staggeredOnObjectUpdated.Add((sender, e)); };
        ;
        objectProcessor.ObjectRemoved += (sender, e) => { _staggeredOnObjectRemoved.Add((sender, e)); };
        tuioServer.AddDataProcessor(objectProcessor);
    }
}