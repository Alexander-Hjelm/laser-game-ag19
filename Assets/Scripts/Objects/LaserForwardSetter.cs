using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserForwardSetter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var laser = GetComponentInChildren<Laser>();
        laser.SetForward(GetComponent<Transform>().forward);
    }
}
