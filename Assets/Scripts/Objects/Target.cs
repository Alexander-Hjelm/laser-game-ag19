using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The target that must be hit by a laser to progress
public class Target : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private Color _color;
    [SerializeField] private MeshRenderer _coreMeshRenderer;    // The core mesh that will change color depeding on what color was assigned

    private Material _onMaterial;
    private Material _offMaterial;

    private bool _keepMaterialOnThisFrame = false;

    private void Start()
    {
        GameManager.RegisterTarget(_id);

        _offMaterial = new Material(Shader.Find("Standard"));
        _onMaterial = _coreMeshRenderer.material;
        SetMaterialOff();
    }

    private void LateUpdate()
    {
        if(!_keepMaterialOnThisFrame)
        {
            SetMaterialOff();
        }
        _keepMaterialOnThisFrame = false;
    }

    public void SetMaterialOn()
    {
        SetMaterial(_onMaterial);
    }

    public void SetMaterialOff()
    {
        SetMaterial(_offMaterial);
    }

    public void KeepMaterialOnThisFrame()
    {
        SetMaterialOn();
        _keepMaterialOnThisFrame = true;
    }

    private void SetMaterial(Material material)
    {
        _coreMeshRenderer.material = material;
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
