using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Laser Switch is only activated as long as a laser is hitting it.
 */
public class LaserSwitch : Switch
{
    private int shutDownCount;
    
    // Start is called before the first frame update
    void Start()
    {
        shutDownCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        shutDownCount++;
        if (shutDownCount >= 2) // Turn switchable off if laser isn't hitting the switch after 2 updates.
            switchable.SwitchTo(false);
    }

    public override void ActivateSwitch() {
        switchable.SwitchTo(true);
        shutDownCount = 0;
    }
}
