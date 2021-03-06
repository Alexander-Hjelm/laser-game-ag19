﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public ObjectType Type;
    private List<long> _CurrentObjects = new List<long>();

    LineRenderer[] fxLineRenderers = new LineRenderer[4];
    TextMesh fxTextMesh;

    private void Awake()
    {
        GraphicsSetup();
    }

    private void GraphicsSetup()
    {

        // Graphics Setup (Only once, assuming the zone is stationary)
        for(int x=0; x<4; x++)
        {
            GameObject lrChild = new GameObject();
            lrChild.name = "LineRendererChild";
            lrChild.transform.parent = transform;
            lrChild.transform.position = transform.position;
            LineRenderer lineRenderer = lrChild.AddComponent(typeof(LineRenderer)) as LineRenderer;
            lineRenderer.SetVertexCount(2);
            lineRenderer.numCapVertices = 5;
            lineRenderer.textureMode = LineTextureMode.Tile;

            // Material setup
            Color color = Color.white;
            lineRenderer.material = new Material(Shader.Find("Unlit/LaserUnlitShader"));
            lineRenderer.material.color = color*1.3f; // Multiply by HDR intensity
            Texture mainTex = Resources.Load<Texture>("Textures/area_border_main");
            lineRenderer.material.SetTexture("_MainTex", mainTex);
            lineRenderer.material.SetFloat("_MainScrollSpeed", 20);
            lineRenderer.material.SetFloat("_NoiseScaleX", 10);
            lineRenderer.material.SetFloat("_NoiseScaleY", 8);
            lineRenderer.material.SetFloat("_NoiseAmount", 0.2f);
            lineRenderer.material.mainTextureScale = new Vector2(0.2f, 0.6f);
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.2f));

            fxLineRenderers[x] = lineRenderer;
        }

        // Set line renderer positions
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 center = transform.position + boxCollider.center;
        float offsetX = boxCollider.size.x * transform.localScale.x / 2;
        float offsetZ = boxCollider.size.z * transform.localScale.z / 2;
        float padding = 0.25f;
        fxLineRenderers[0].SetPosition(0, center + new Vector3(offsetX - padding, 0f, offsetZ));
        fxLineRenderers[0].SetPosition(1, center + new Vector3(-offsetX + padding, 0f, offsetZ));
        fxLineRenderers[1].SetPosition(0, center + new Vector3(-offsetX, 0f, offsetZ - padding));
        fxLineRenderers[1].SetPosition(1, center + new Vector3(-offsetX, 0f, -offsetZ + padding));
        fxLineRenderers[2].SetPosition(0, center + new Vector3(-offsetX + padding, 0f, -offsetZ));
        fxLineRenderers[2].SetPosition(1, center + new Vector3(offsetX - padding, 0f, -offsetZ));
        fxLineRenderers[3].SetPosition(0, center + new Vector3(offsetX, 0f, -offsetZ + padding));
        fxLineRenderers[3].SetPosition(1, center + new Vector3(offsetX, 0f, offsetZ - padding));

        // Add text mesh
        TextMesh textMesh = GetComponentInChildren<TextMesh>();
        textMesh.transform.position = transform.position;
        textMesh.transform.rotation = Quaternion.LookRotation(-Vector3.up, Vector3.forward);
        textMesh.transform.localScale = new Vector3(1/transform.localScale.x, 1/transform.localScale.y, 1/transform.localScale.z);
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.characterSize = 0.1f;
        textMesh.fontSize = 92;
        textMesh.text = "Place " + System.Enum.GetName(typeof(ObjectType), Type) + " here...";
        fxTextMesh = textMesh;
    }

    public void SetGraphicsEnabled(bool on)
    {
        fxTextMesh.gameObject.SetActive(on);
        foreach(LineRenderer lineRenderer in fxLineRenderers)
        {
            lineRenderer.gameObject.SetActive(on);
        }
    }

    public void SetFxColor(Color color)
    {
        fxTextMesh.color = color;
        foreach(LineRenderer lineRenderer in fxLineRenderers)
        {
            lineRenderer.material.color = color*1.3f; // Multiply by HDR intensity
        }
    }

    public void Place(long id)
    {
        _CurrentObjects.Add(id);
        OnEnter(GameManager.GetSpawnedObject(id));
    }

    /*
     * Update the object with id, removes it from the zone if it no longer is within
     * returns true if removed, false otherwise
     */
    public bool UpdateGameObject(long id)
    {
        var position = GameManager.GetSpawnedObject(id).transform.position;
        if (IsInside(position)) return true;
        _CurrentObjects.Remove(id);
        return false;
    }

    public bool Contains(long gameId)
    {
        return _CurrentObjects.Contains(gameId);
    }

    /**
     * Called whenever an appropriate GameObject enters this zone.
     */
    protected virtual void OnEnter(GameObject other)
    {
    }

    public bool IsInside(Vector3 point)
    {
        var closest = GetComponent<Collider>().ClosestPoint(point);
        var dist = (point - closest).magnitude;
        return dist >= -Mathf.Epsilon && dist < Mathf.Epsilon;
    }
}
