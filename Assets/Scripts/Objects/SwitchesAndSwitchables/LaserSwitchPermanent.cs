using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Laser Switch turns on permanently when a laser hits it.
 */
public class LaserSwitchPermanent : Switch
{
    private bool on;

    public override void ActivateSwitch() {
        if (!on) {
            switchable.SwitchTo(true);
            on = true;
        }
    }
}
