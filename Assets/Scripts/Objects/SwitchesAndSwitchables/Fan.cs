using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : Switchable
{
    private bool on;
    private float distanceMoved;
    private float startTime;

    // The object to push
    public Transform pushObject;
    // The direction to push
    public Vector3 pushDirection;
    // The distance to push
    public float distance;
    // How fast it moves in units/s
    public float strength = 1;
    // How long time the whole thing should take in seconds
    public float duration = 1;

    // Start is called before the first frame update
    void Start()
    {
        on = false;
        distanceMoved = 0f;
        startTime = 0f;
    }

    void Update()
    {
        if (on && distanceMoved < distance)
        {
            if (Math.Abs(startTime) < float.Epsilon)
            {
                // if we start moving here
                startTime = Time.time;
            }
            // Push object
            float t = (Time.time - startTime) / duration;
            var pushVec = Time.deltaTime * distance * Mathf.SmoothStep(0, strength, t) * pushDirection.normalized;
            pushObject.Translate(pushVec);
            distanceMoved += pushVec.magnitude;
        }
    }

    public override void SwitchTo(bool switchTo) {
        on = switchTo;
    }
}
