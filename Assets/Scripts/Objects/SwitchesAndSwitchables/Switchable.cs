using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Abstract class that all switchables (doors etc) should inherit
 */
public abstract class Switchable : MonoBehaviour
{
    public abstract void SwitchTo(bool switchTo);
}
