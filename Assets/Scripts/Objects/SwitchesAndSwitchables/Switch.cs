using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Switch : MonoBehaviour
{
    [SerializeField]
    protected Switchable switchable;

    public abstract void ActivateSwitch();
}
