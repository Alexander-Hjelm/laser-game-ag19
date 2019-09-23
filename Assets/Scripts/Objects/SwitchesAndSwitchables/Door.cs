using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Switchable
{
    private bool open;
    private float height;

    [SerializeField]
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        open = false;
        height = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!open && transform.position.y <= height) { //Close door over time
            transform.position += new Vector3(0, speed, 0);
            if (transform.position.y > height)
                transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
        else if(open && transform.position.y >= -height * 2) { //Open door over time
            transform.position -= new Vector3(0, speed, 0);
            if (transform.position.y < -height * 2)
                transform.position = new Vector3(transform.position.x, -height * 2, transform.position.z);
        }
    }

    public override void SwitchTo(bool switchTo) {
        open = switchTo;
    }
}
