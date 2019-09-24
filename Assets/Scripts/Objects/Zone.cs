using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Objects Type;
    public int MaxInZone;
    private List<int> _CurrentObjects = new List<int>();

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
