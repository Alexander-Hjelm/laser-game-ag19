using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    [SerializeField] private float _speed;

    void Update()
    {
        transform.Rotate(0f, _speed*Time.deltaTime, 0f, Space.World);
    }
}
