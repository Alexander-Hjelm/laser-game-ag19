using UnityEngine;
using System.Collections;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using TUIOsharp.Entities;
using System;

namespace Assets
{
    class TUIOInput : MonoBehaviour
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
        }

        private void OnDestroy()
        {
            tuioServer.Disconnect();
            tuioServer = null;
            instance = null;
        }

        private void ListenForTUIO()
        {
            UnityEngine.Debug.Log(string.Format("TUIO listening on port {0}.", port));

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
            throw new NotImplementedException();
        }

        private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnObjectAdded(object sender, TuioObjectEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnBlobRemoved(object sender, TuioBlobEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnBlobUpdated(object sender, TuioBlobEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnBlobAdded(object sender, TuioBlobEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCursorRemoved(object sender, TuioCursorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCursorUpdated(object sender, TuioCursorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCursorAdded(object sender, TuioCursorEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
