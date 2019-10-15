using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassWall : MonoBehaviour
{
    [SerializeField] private Color _color;

    void Start()
    {
        // Material setup
        Material newMat = GetComponent<MeshRenderer>().material;
        newMat.color = new Color(_color.r, _color.g, _color.b, 0.5f);
        GetComponent<MeshRenderer>().material = newMat;
    }

    public Color GetColor()
    {
        return _color;
    }
}
