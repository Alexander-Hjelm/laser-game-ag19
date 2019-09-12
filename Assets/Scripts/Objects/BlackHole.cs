using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private float gravityConstant; // How strong gravity is for this black hole
    [SerializeField] private float absorptionRadius; // How big is the radius for when the laser gets completely eaten by the hole

    public float getGravityConstant () {
        return gravityConstant;
    }

    public float getAbsorptionRadius () {
        return absorptionRadius;
    }
}
