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
        Vector3 nextHit = transform.position;
        Vector3 nextDir = forward;
        List<Vector3> points = new List <Vector3>();
        points.Add(nextHit);
        
        while(Physics.Raycast(nextHit, nextDir, out raycastHit, LayerMask.GetMask("Mirror"))){
            Vector3 point = raycastHit.point;
            Vector3 normal = raycastHit.normal;
            Vector3 v = point - nextHit;
            Debug.Log(point);
            Vector3 r = 2*v - 2*Vector3.Project(v, normal);
            nextHit = point;
            nextDir = r;
            Debug.Log(r);
            points.Add(point);

            
        }
        lineRender.SetVertexCount(points.Count);
        for(int i = 0; i< points.Count; i++){
            lineRender.SetPosition(i, points[i]); 
        }
        
        


        
    }



}
