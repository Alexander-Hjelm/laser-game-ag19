using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The target that must be hit by a laser to progress
public class Target : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private Color _color;

    private void Start()
    {
        GameManager.RegisterTarget(_id);

        MeshRenderer meshRenderer = GetComponent <MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = _color;
    }

    public int GetId()
    {
        return _id;
    }

    public Color GetColor()
    {
        return _color;
    }
}
