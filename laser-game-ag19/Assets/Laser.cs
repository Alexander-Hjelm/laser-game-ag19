using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{   
    [SerializeField]
    Vector3 forward;
    LineRenderer lineRender;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello");
        GetComponent <LineRenderer>();
        lineRender = GetComponent <LineRenderer>();



    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit raycastHit;
        bool hit = Physics.Raycast(transform.position, forward, out raycastHit, LayerMask.GetMask("Mirror"));
        if(hit){
            Vector3 point = raycastHit.point;
            Vector3 normal = raycastHit.normal;
            Vector3 v = point - transform.position;
            Vector3 r = 2*v - 2*Vector3.Project(v, normal);

            lineRender.SetVertexCount(3);
            lineRender.SetPosition(0, transform.position); 
            lineRender.SetPosition(1, point);
            lineRender.SetPosition(2, r);
        }
        
        


        
    }



}
