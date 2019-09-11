using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private float gravityConstant; // How strong gravity is for this black hole (reasonable value is 0.5)
    [SerializeField] private float absorptionRadius; // How big is the radius for when the laser gets completely eaten by the hole

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getGravityConstant () {
        return gravityConstant;
    }

    public float getAbsorptionRadius () {
        return absorptionRadius;
    }
}
