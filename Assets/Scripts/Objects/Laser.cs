using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{   
    // The original direction of the laser
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
        Vector3 nextHit = transform.position;   // The next hit point from the raycast
        Vector3 nextDir = forward;              // The next forward direction from the raycast
        List<Vector3> points = new List <Vector3>();    // All points that the line renderer should render
        points.Add(nextHit);    // Add start position to LineRenderer
        bool laserShouldStop = false;   // Should the laser stop before the next raycast?

        // Main raycast loop
        // Raycasts will start from the successive hit points, and continue while the laser is hitting objects
        // nextHit and nextDir are updated internally for every hit
        while(Physics.Raycast(nextHit, nextDir, out raycastHit, LayerMask.GetMask("Laser Hittable"))){
            switch(raycastHit.collider.tag)
            {
                case "Mirror":
                    Vector3 point = raycastHit.point;
                    Vector3 normal = raycastHit.normal;

                    // Do some projection to find the direction of the reflected laser
                    Vector3 v = point - nextHit;
                    Vector3 r = v - 2*Vector3.Project(v, normal);

                    // Update the origin and direction of the next laser
                    nextHit = point;
                    nextDir = r;

                    // Add the hit point to the LineRenderer
                    points.Add(point);
                    break;

                case "Target":
                    // Get Target ID
                    int targetId = raycastHit.collider.GetComponent<Target>().GetId();

                    // Notify the GameManager that the Target has been hit on this frame
                    GameManager.HitTarget(targetId);
                    nextHit = raycastHit.point;

                    // Laser should not continue to raycast
                    laserShouldStop = true;
                    break;
                case "Wall":
                    // Set final hitpoint 
                    nextHit = raycastHit.point;

                    // Laser should not continue to raycast
                    laserShouldStop = true;
                    break;


                // TODO: Add more cases here for different objects that the laser can interact with

            }

            if(laserShouldStop) break;
        }
        // Did the laser stop or does it continue indefinitely?
        if (laserShouldStop)
            points.Add(nextHit);
        else
            points.Add(nextHit + nextDir*10000);

        // Render the laser
        lineRender.SetVertexCount(points.Count);
        for(int i = 0; i< points.Count; i++){
            lineRender.SetPosition(i, points[i]); 
        }
    }

}
