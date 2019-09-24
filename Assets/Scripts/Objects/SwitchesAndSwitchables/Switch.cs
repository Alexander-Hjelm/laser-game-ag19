using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Abstract Class that all switches should inherit
 */
public abstract class Switch : MonoBehaviour
{
    [SerializeField]
    protected Switchable switchable;

    public abstract void ActivateSwitch();
}
