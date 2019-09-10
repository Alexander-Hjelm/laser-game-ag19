using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{   
    // The original direction of the laser
    [SerializeField] private Vector3 forward;

    // The color of the laser
    [SerializeField] private Color color;

    private LineRenderer lineRender;

    // The prism that spawned this laser, if any
    private Prism _rootPrism;

    // 
    private Collider currentHole;

    // Start is called before the first frame update
    void Start()
    {
        // Set LineRenderer reference
        lineRender = GetComponent <LineRenderer>();
        // Set material color
        lineRender.material = new Material(Shader.Find("Unlit/Color"));
        lineRender.material.color = color;
    }

    void Update()
    {
        RaycastHit raycastHit;
        Vector3 nextHit = transform.position;   // The next hit point from the raycast
        Vector3 nextDir = forward;              // The next forward direction from the raycast
        List<Vector3> points = new List <Vector3>();    // All points that the line renderer should render
        points.Add(nextHit);    // Add start position to LineRenderer
        bool laserShouldStop = false;   // Should the laser stop before the next raycast?

        // TODO move these
        // Variables that define constants like how often the line breaks and gravity constants etc 
        bool inHole = false; //is the raycast in a black hole?
        float gravityConstant = 50f;
        float deltaLine = 0.1f;
        float maxDistance = 1000;

        // Main raycast loop
        // Raycasts will start from the successive hit points, and continue while the laser is hitting objects
        // nextHit and nextDir are updated internally for every hit
        while (Physics.Raycast(nextHit, nextDir, out raycastHit, maxDistance, LayerMask.GetMask("Laser Hittable")) || inHole){
            if(raycastHit.collider != null)
            switch (raycastHit.collider.tag)
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
                    Color targetColor = raycastHit.collider.GetComponent<Target>().GetColor();

                    if(targetColor == color)
                    {
                        // Notify the GameManager that the Target has been hit on this frame
                        GameManager.HitTarget(targetId);
                    }

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

                case "Prism":
                    if(raycastHit.collider.GetComponent<Prism>() == _rootPrism)
                    {
                        // The prism we collided with was the root prism (that spawned this laser), so ignore it and continue
                        // along the same line until we have exited its collision bounds
                        nextHit = nextHit + nextDir*0.1f;
                    }
                    else
                    {
                        // We hit a new prism. The laser splits and stops here
                        
                        // Set final hitpoint 
                        nextHit = raycastHit.point;

                        // Laser should not continue to raycast
                        laserShouldStop = true;

                        // Notify that this laser should split at this point
                        GameManager.NotifyLaserShouldSplit(this, nextHit, nextDir, raycastHit.collider.GetComponent<Prism>());
                    }

                    break;
                case "Hole":
                    Debug.Log("Hit hole");
                    inHole = !inHole;

                    currentHole = raycastHit.collider;

                    nextDir = BlackHoleDealer(raycastHit.point, nextDir, gravityConstant);

                    nextHit = raycastHit.point + nextDir * deltaLine;

                    points.Add(raycastHit.point);

                    break;

                // TODO: Add more cases here for different objects that the laser can interact with

            }

            if (inHole) {
                Debug.Log("Still in hole");

                points.Add(nextHit);

                nextDir = BlackHoleDealer(nextHit, nextDir, gravityConstant);

                nextHit = nextHit + nextDir * deltaLine;
                maxDistance = nextDir.magnitude * deltaLine;

                if ((nextHit - currentHole.transform.position).magnitude < 1 || (nextHit - currentHole.transform.position).magnitude > 10) inHole = false;
            }  else
                maxDistance = 1000;

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

    private Vector3 BlackHoleDealer(Vector3 point, Vector3 currentTrajectory, float gravityConstant) {
        // Center of the black hole
        Vector3 blackHolePos = currentHole.transform.position;
        //Vector3 point = raycastHit.point;

        Vector3 distance = (blackHolePos - point);
        Vector3 gravityPull = distance.normalized * (gravityConstant / Mathf.Pow(distance.magnitude, 2));

        currentTrajectory += gravityPull;
        currentTrajectory.y = 0;
        return currentTrajectory;
    }

    // Set the color of this laser
    // NOTE: Must be called before start() on the instanced laser object, otherwise it will have no effect
    public void SetColor(Color color)
    {
        this.color = color;
    }

    // Set the forward vector of this laser
    public void SetForward(Vector3 forward)
    {
        this.forward = forward;
    }

    // Set the root prism object of this laser
    // NOTE: Must be called before start() on the instanced laser object, otherwise it will have no effect
    public void SetRootPrism(Prism prism)
    {
        _rootPrism = prism;
    }

    public Color GetColor()
    {
        return this.color;
    }

}
