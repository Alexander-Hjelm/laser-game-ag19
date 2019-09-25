using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserZone : Zone
{
    public Color LaserColor;
    
    private void Start()
    {
        SetFxColor(LaserColor);
    }

    public override void OnEnter(GameObject other)
    {
        var laser = other.GetComponentInChildren<Laser>();
        laser.SetColor(LaserColor);
        var forward = other.GetComponent<Transform>().forward;
        laser.SetForward(forward);
    }
}
