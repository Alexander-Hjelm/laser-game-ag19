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

    // Start is called before the first frame update
    void Start()
    {
        // Set LineRenderer reference
        lineRender = GetComponent <LineRenderer>();
        // Material setup
        lineRender.material = new Material(Shader.Find("Unlit/LaserUnlitShader"));
        lineRender.material.color = color;
        Texture mainTex = Resources.Load<Texture>("Textures/laser_main");
        Debug.Log(mainTex);
        lineRender.material.SetTexture("_MainTex", mainTex);
    }

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
