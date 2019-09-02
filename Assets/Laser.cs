using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{   
    [SerializeField] Vector3 forward;

    LineRenderer lineRender;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello");
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
        bool laserShouldStop = false;
        
        while(Physics.Raycast(nextHit, nextDir, out raycastHit, LayerMask.GetMask("Laser Hittable"))){
            switch(raycastHit.collider.tag)
            {
                case "Mirror":
                    Vector3 point = raycastHit.point;
                    Vector3 normal = raycastHit.normal;
                    Vector3 v = point - nextHit;
                    Vector3 r = v - 2*Vector3.Project(v, normal);
                    nextHit = point;
                    nextDir = r;
                    points.Add(point);
                    break;

                case "Target":
                    int targetId = raycastHit.collider.GetComponent<Target>().GetId();
                    GameManager.HitTarget(targetId);
                    nextHit = raycastHit.point;
                    laserShouldStop = true;
                    break;

                // TODO: Add more cases here for different objects that the laser can interact with

            }

            if(laserShouldStop) break;
        }
        if (laserShouldStop)
            points.Add(nextHit);
        else
            points.Add(nextHit + nextDir*20);
        lineRender.SetVertexCount(points.Count);
        for(int i = 0; i< points.Count; i++){
            lineRender.SetPosition(i, points[i]); 
        }
    }

}
