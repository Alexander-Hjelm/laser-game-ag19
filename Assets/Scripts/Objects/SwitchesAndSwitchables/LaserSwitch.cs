using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Laser Switch is only activated as long as a laser is hitting it.
 */
public class LaserSwitch : Switch
{
    private int shutDownCount;
    private int startUpCount;

    [SerializeField]
    private int startUpTime;
    [SerializeField]
    private int shutDownTime;
    
    // Start is called before the first frame update
    void Start()
    {
        shutDownCount = 0;
        startUpCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        shutDownCount++;
        if (shutDownCount >= shutDownTime) {// Turn switchable off if laser isn't hitting the switch after 2 updates.
            switchable.SwitchTo(false);
            startUpCount = 0;
        }
        if (startUpCount >= startUpTime) {
            switchable.SwitchTo(true);
        }
    }

    public override void ActivateSwitch() {
        startUpCount++;
        shutDownCount = 0;
    }
}
