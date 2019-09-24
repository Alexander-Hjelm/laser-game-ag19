﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Objects Type;
    public int MaxInZone;
    private List<int> _CurrentObjects = new List<int>();

    private void Start()
    {
        GraphicsSetup();
    }

    private void GraphicsSetup()
    {
        LineRenderer[] addedLineRenderers = new LineRenderer[4];

        // Graphics Setup (Only once, assuming the zone is stationary)
        for(int x=0; x<4; x++)
        {
            GameObject lrChild = new GameObject();
            lrChild.name = "LineRendererChild";
            lrChild.transform.parent = transform;
            lrChild.transform.position = transform.position;
            LineRenderer lineRenderer = lrChild.AddComponent(typeof(LineRenderer)) as LineRenderer;
            lineRenderer.SetVertexCount(2);

            // Material setup
            Color color = Color.white;
            lineRenderer.material = new Material(Shader.Find("Unlit/LaserUnlitShader"));
            lineRenderer.material.color = color*1.1f; // Multiply by HDR intensity
            Texture mainTex = Resources.Load<Texture>("Textures/laser_main");
            lineRenderer.material.SetTexture("_MainTex", mainTex);
            lineRenderer.material.SetFloat("_MainScrollSpeed", 20);
            lineRenderer.material.SetFloat("_NoiseScaleX", 10);
            lineRenderer.material.SetFloat("_NoiseScaleY", 8);
            lineRenderer.material.SetFloat("_NoiseAmount", 0.2f);
            lineRenderer.material.mainTextureScale = new Vector2(0.05f, 0.6f);
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.2f));

            addedLineRenderers[x] = lineRenderer;
        }

        // Set line renderer positions
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 center = transform.position + boxCollider.center;
        float offsetX = boxCollider.size.x * transform.localScale.x / 2;
        float offsetZ = boxCollider.size.z * transform.localScale.z / 2;
        addedLineRenderers[0].SetPosition(0, center + new Vector3(offsetX, 0f, offsetZ));
        addedLineRenderers[0].SetPosition(1, center + new Vector3(-offsetX, 0f, offsetZ));
        addedLineRenderers[1].SetPosition(0, center + new Vector3(-offsetX, 0f, offsetZ));
        addedLineRenderers[1].SetPosition(1, center + new Vector3(-offsetX, 0f, -offsetZ));
        addedLineRenderers[2].SetPosition(0, center + new Vector3(-offsetX, 0f, -offsetZ));
        addedLineRenderers[2].SetPosition(1, center + new Vector3(offsetX, 0f, -offsetZ));
        addedLineRenderers[3].SetPosition(0, center + new Vector3(offsetX, 0f, -offsetZ));
        addedLineRenderers[3].SetPosition(1, center + new Vector3(offsetX, 0f, offsetZ));

        // Add text mesh
        GameObject textChild = new GameObject();
        textChild.name = "TextMeshChild";
        textChild.transform.parent = transform;
        textChild.transform.position = transform.position;
        TextMesh textMesh = textChild.AddComponent(typeof(TextMesh)) as TextMesh;
        textMesh.transform.rotation = Quaternion.LookRotation(-Vector3.up, Vector3.forward);
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.text = "Place " + System.Enum.GetName(typeof(Objects), Type) + " here...";

        
    }

    /**
     * Returns true if object has been placed within zone or is still within zone
     */
    public bool TryPlace(int id, Vector3 position)
    {
        var contains = _CurrentObjects.Contains(id);
        var isInside = IsInside(position);
        if (_CurrentObjects.Contains(id))
        {
            if (IsInside(position)) return true;

            _CurrentObjects.Remove(id);
            return false;

        }

        // If there is no place left in zone, or we are not inside the zone return false
        if (_CurrentObjects.Count >= MaxInZone || !IsInside(position)) return false;

        _CurrentObjects.Add(id);
        return true;

    }

    /**
     * Called whenever an appropriate GameObject enters this zone.
     */
    public virtual void OnEnter(GameObject other)
    {

    }

    public bool IsInside(Vector3 point)
    {
        var closest = GetComponent<Collider>().ClosestPoint(point);
        var dist = (point - closest).magnitude;
        return dist >= -Mathf.Epsilon && dist < Mathf.Epsilon;
    }
}