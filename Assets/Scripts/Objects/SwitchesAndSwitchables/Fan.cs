using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : Switchable
{
    private bool on;
    private float elapsedTime;
    private float startTime;
    private Vector3 startPos;

    // The object to push
    public Transform pushObject;
    // The point to push to
    public Vector3 pushTo;
    // How long time the whole thing should take in seconds
    public float duration = 1;

    // Start is called before the first frame update
    void Start()
    {
        on = false;
        elapsedTime = 0f;
        startTime = 0f;
    }

    void Update()
    {
        if (on && elapsedTime <= duration)
        {
            if (Math.Abs(startTime) < float.Epsilon)
            {
                // if we start moving here
                startTime = Time.time;
                startPos = pushObject.position;
            }
            // Push object
            elapsedTime = (Time.time - startTime);
            float t = elapsedTime / duration;
            pushObject.position = Vector3.Lerp(startPos, pushTo, Mathf.SmoothStep(0, 1, t));
        }
        else {
            //Reset values
            Start();
        }
    }

    public override void SwitchTo(bool switchTo) {
        on = switchTo;
    }
}
