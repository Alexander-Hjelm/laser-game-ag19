using System.Collections;
using System.Collections.Generic;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using UnityEngine;

public class TUIOInput : MonoBehaviour
{
    private static TUIOInput instance = null;

    private int port = 3333;
    private TuioServer tuioServer;

    private void Awake()
    {
        if (instance != null)
        {
            throw new UnityException("Two instances of a TUIOInput was found. Please remove one.");
        }
        ListenForTUIO();
        instance = this;
        Debug.Log("Started TUIO Client");
    }

    private void OnDestroy()
    {
        tuioServer.Disconnect();
        tuioServer = null;
        instance = null;
    }

    private void ListenForTUIO()
    {
        Debug.Log(string.Format("TUIO listening on port {0}.", port));

        // tuio
        tuioServer = new TuioServer(port);

        CursorProcessor cursorProcessor = new CursorProcessor();
        cursorProcessor.CursorAdded += OnCursorAdded;
        cursorProcessor.CursorUpdated += OnCursorUpdated;
        cursorProcessor.CursorRemoved += OnCursorRemoved;

        BlobProcessor blobProcessor = new BlobProcessor();
        blobProcessor.BlobAdded += OnBlobAdded;
        blobProcessor.BlobUpdated += OnBlobUpdated;
        blobProcessor.BlobRemoved += OnBlobRemoved;

        ObjectProcessor objectProcessor = new ObjectProcessor();
        objectProcessor.ObjectAdded += OnObjectAdded;
        objectProcessor.ObjectUpdated += OnObjectUpdated;
        objectProcessor.ObjectRemoved += OnObjectRemoved;

        // listen...
        tuioServer.Connect();

        tuioServer.AddDataProcessor(cursorProcessor);
        tuioServer.AddDataProcessor(blobProcessor);
        tuioServer.AddDataProcessor(objectProcessor);
    }

    private void OnObjectRemoved(object sender, TuioObjectEventArgs e)
    {
        
        Debug.Log("OnObjectRemoved");
    }

    private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
    {
        Debug.Log($"OnObjectUpdated, classId={e.Object.ClassId}, Id={e.Object.Id}");
    }

    private void OnObjectAdded(object sender, TuioObjectEventArgs e)
    {
        Debug.Log($"OnObjectAdded, classId={e.Object.ClassId}, Id={e.Object.Id}");
    }

    private void OnBlobRemoved(object sender, TuioBlobEventArgs e)
    {
        Debug.Log("OnBlobRemoved");
    }

    private void OnBlobUpdated(object sender, TuioBlobEventArgs e)
    {
        Debug.Log("OnBlobUpdated");
    }

    private void OnBlobAdded(object sender, TuioBlobEventArgs e)
    {
        Debug.Log("OnBlobAdded");
    }

    private void OnCursorRemoved(object sender, TuioCursorEventArgs e)
    {
        Debug.Log("OnCursorRemoved");
    }

    private void OnCursorUpdated(object sender, TuioCursorEventArgs e)
    {
        Debug.Log("OnCursorUpdated");
    }

    private void OnCursorAdded(object sender, TuioCursorEventArgs e)
    {
        Debug.Log("OnCursorAdded");
    }
}

