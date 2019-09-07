using System;
using System.Collections;
using System.Collections.Generic;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using UnityEngine;

public class TUIOInput : MonoBehaviour
{
    private static TUIOInput instance = null;

    public static event EventHandler<TuioObjectEventArgs> OnObjectAdded;
    public static event EventHandler<TuioObjectEventArgs> OnObjectRemoved;
    public static event EventHandler<TuioObjectEventArgs> OnObjectUpdated;

    // We want to stagger all events to make them run on the main unity thread
    private List<(object, TuioObjectEventArgs)> staggeredOnObjectAdded;
    private List<(object, TuioObjectEventArgs)> staggeredOnObjectRemoved;
    private List<(object, TuioObjectEventArgs)> staggeredOnObjectUpdated;

    private int port = 3333;
    private TuioServer tuioServer;

    private void Awake()
    {
        if (instance != null)
        {
            throw new UnityException("Two instances of a TUIOInput was found. Please remove one.");
        }
        staggeredOnObjectAdded = new List<(object, TuioObjectEventArgs)>();
        staggeredOnObjectRemoved = new List<(object, TuioObjectEventArgs)>();
        staggeredOnObjectUpdated = new List<(object, TuioObjectEventArgs)>();
        ListenForTUIO();
        instance = this;
        Debug.Log("Started TUIO Client");
    }

    private void OnDestroy()
    {
        tuioServer.Disconnect();
        tuioServer = null;
        instance = null;
        OnObjectAdded = null;
        OnObjectRemoved = null;
        OnObjectUpdated = null;
    }

    private void Update()
    {
        staggeredOnObjectAdded.RemoveAll((args) =>
        {
            OnObjectAdded?.Invoke(args.Item1, args.Item2);
            return true;
        });
        staggeredOnObjectRemoved.RemoveAll((args) =>
        {
            OnObjectRemoved?.Invoke(args.Item1, args.Item2);
            return true;
        });
        staggeredOnObjectUpdated.RemoveAll((args) =>
        {
            OnObjectUpdated?.Invoke(args.Item1, args.Item2);
            return true;
        });
    }

    private void ListenForTUIO()
    {
        // tuio
        tuioServer = new TuioServer(port);
        tuioServer.Connect();

        Debug.Log(string.Format("TUIO listening on port {0}.", port));

        // Add Object Processor
        ObjectProcessor objectProcessor = new ObjectProcessor();
        objectProcessor.ObjectAdded += (sender, e) =>
        {
            staggeredOnObjectAdded.Add((sender, e));
        };
        objectProcessor.ObjectUpdated += (sender, e) =>
        {
            staggeredOnObjectUpdated.Add((sender, e));
        }; ;
        objectProcessor.ObjectRemoved += (sender, e) =>
        {
            staggeredOnObjectRemoved.Add((sender, e));
        };
        tuioServer.AddDataProcessor(objectProcessor);
    }
}

