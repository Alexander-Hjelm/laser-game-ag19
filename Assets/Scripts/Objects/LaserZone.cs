using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserZone : Zone
{
    public override void OnEnter(GameObject other)
    {
        Debug.Log("Collision");
    }
}
