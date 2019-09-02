using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The target that must be hit by a laser to progress
public class Target : MonoBehaviour
{
    [SerializeField] private int _id;

    private void Start()
    {
        GameManager.RegisterTarget(_id);
    }

    public int GetId()
    {
        return _id;
    }
}
