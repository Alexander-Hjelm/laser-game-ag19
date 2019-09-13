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
        lineRender.material.color = color*1.35f; // Multiply by HDR intensity
        Texture mainTex = Resources.Load<Texture>("Textures/laser_main");
        lineRender.material.SetTexture("_MainTex", mainTex);
        lineRender.material.SetFloat("_MainScrollSpeed", 20);
        lineRender.material.SetFloat("_NoiseScaleX", 10);
        lineRender.material.SetFloat("_NoiseScaleY", 8);
        lineRender.material.SetFloat("_NoiseAmount", 0.2f);
        lineRender.material.mainTextureScale = new Vector2(0.05f, 0.6f);
        lineRender.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.2f));
    }

    void Update()
    {
        RaycastHit raycastHit;
        Vector3 nextHit = transform.position;   // The next hit point from the raycast
        Vector3 nextDir = forward;              // The next forward direction from the raycast
        List<Vector3> points = new List <Vector3>();    // All points that the line renderer should render
        points.Add(nextHit);    // Add start position to LineRenderer
        bool laserShouldStop = false;   // Should the laser stop before the next raycast?
        float maxDistance = 1000; // Max Distance of raycast
        float blackHoleLineDelta = 1f; // How often the laser bends in a black hole (smaller = more times)
        bool inHole = false; // Is the raycast in a black hole?
        BlackHole currentHole = null; // The black hole the raycast is currently in
        float currentHoleRadius = 0.0f; // The radius of the black hole the raycast is currently in
        int holeIterations = 0; //Number of times we've cast a ray in a hole

        // Check if a laser is spawned inside of a black hole
        Collider[] startingIn = Physics.OverlapSphere(nextHit, 0, LayerMask.GetMask("Laser Hittable"));
        foreach (Collider c in startingIn) {
            if(c.tag == "Hole") {
                inHole = true;
                currentHole = c.GetComponent<BlackHole>();
                currentHoleRadius = c.GetComponent<SphereCollider> ().radius * c.transform.localScale.x;
                maxDistance = nextDir.magnitude * blackHoleLineDelta;
                break;
            }
        }

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
                    nextDir = r.normalized * nextDir.magnitude;

                    // Notify game manager that the laser has hit a wall
                    // The GameManager will proceed to spawn a laser hit particle system
                    GameManager.NotifyLaserHit(this, nextHit, normal);

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

                    // Notify game manager that the laser has hit a wall
                    // The GameManager will proceed to spawn a laser hit particle system
                    GameManager.NotifyLaserHit(this, nextHit, -nextDir);

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
                    // Laser is now being affected by the black hole
                    inHole = true;

                    // Set up the properties of the black hole we are currently in
                    currentHole = raycastHit.collider.GetComponent<BlackHole> ();
                    currentHoleRadius = (currentHole.transform.position - raycastHit.point).magnitude;

                    // Set the entering point of the black hole to the lines and the startpoint for the next raycast
                    nextHit = raycastHit.point;
                    points.Add(nextHit);

                    break;

                // TODO: Add more cases here for different objects that the laser can interact with

            }

            // As long as the laser is within the gravitational field of the black hole it should be curving towards it
            if (inHole) {
                // Calculating the place the new bended raycast will start
                if(nextHit != raycastHit.point) // This is to prevent skipping when colliding with objects
                    nextHit += nextDir * blackHoleLineDelta; //Note that when the laser collides with something (in a black hole) or enters a black hole the raycast will skip a little

                // Calculating the force of gravity based on distance from laser to black hole
                Vector3 distance = (currentHole.transform.position - nextHit);
                Vector3 gravityPull = distance.normalized * (currentHole.getGravityConstant() * blackHoleLineDelta * 10 / Mathf.Pow(distance.magnitude, 2));

                // Adding gravity pull to current trajectory of laser
                nextDir += gravityPull;
                nextDir.y = 0;

                maxDistance = nextDir.magnitude * blackHoleLineDelta; // Making sure Raycast only goes a short distance (as we need to keep bending)

                points.Add(nextHit); // Add the current point to the line renderer

                holeIterations++;

                // If we exit the gravitational field of the black hole
                if ((nextHit - currentHole.transform.position).magnitude > currentHoleRadius) {
                    inHole = false; // No longer in a black hole
                    nextDir.Normalize(); // Make sure the "speed" of the laser is reset
                    maxDistance = 1000; // Make sure the raycast goes far again
                    holeIterations = 0;
                }
               
                // Laser has been absorbed by the black hole
                if ((nextHit - currentHole.transform.position).magnitude < currentHole.getAbsorptionRadius() || holeIterations * blackHoleLineDelta > 2 * currentHoleRadius * Mathf.PI) //Approximation so a laser doesn't sattelite a black hole too often
                    laserShouldStop = true;
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
