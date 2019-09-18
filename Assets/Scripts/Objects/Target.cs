using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The target that must be hit by a laser to progress
public class Target : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private Color _color;
    [SerializeField] private MeshRenderer _coreMeshRenderer;    // The core mesh that will change color depeding on what color was assigned

    private void Start()
    {
        GameManager.RegisterTarget(_id);

        _coreMeshRenderer.material = new Material(Shader.Find("Standard"));
        _coreMeshRenderer.material.color = _color;
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
