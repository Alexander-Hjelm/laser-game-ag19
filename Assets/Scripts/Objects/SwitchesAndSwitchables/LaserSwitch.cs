using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Image _loaderImage;
    
    private void Awake()
    {
        _loaderImage = GetComponentInChildren<Image>();
    }

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

        // Only uses startup time for now. Maybe add shutdown time later?
        _loaderImage.fillAmount = (float)startUpCount / (float)startUpTime;
    }

    public override void ActivateSwitch() {
        startUpCount++;
        shutDownCount = 0;
    }
}
