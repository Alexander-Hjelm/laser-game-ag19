using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // All prefabs that can be spawned
    [SerializeField] private GameObject[] _spawnablePrefabs;

    // Singleton instance
    private static GameManager _instance;

    private static List<int> _targetIds = new List<int>();      // All Target ids that have been registrered
    private static List<int> _hitTargetIds = new List<int>();   // All Targets that have been hit on the current frame

    // All spawned objects by id reference
    private static Dictionary<long, GameObject> _spawnedObjectsById = new Dictionary<long, GameObject>();

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        // Check if all Targets have been hit on this frame. If so, we shuold finish the level
        if (_hitTargetIds.Count == _targetIds.Count)
        {
            Debug.Log("You win!");
        }
        _hitTargetIds.Clear();  // Regardless of if we won or not, clear _hitTargetIds so that it can be rebuilt on the next frame

        // Cheats, spawn prefabs
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnPrefab("Mirror", new Vector3(0f, 0f, -5f), new Vector3(0f, 0f, 1f));
        }
    }

    // Register a Target. Must be called by the Targets themselves on Awake
    public static void RegisterTarget(int id)
    {
        _targetIds.Add(id);
    }

    // Register that a target has been hit.
    public static void HitTarget(int id)
    {
        _hitTargetIds.Add(id);
    }
    
    // Spawn a prefab at a given location, with a given forward vector
    // nameRef must match the prefab's resource name
    // The unique id is returned, which can be used to move, rotate or destroy the object later
    public static long SpawnPrefab(string nameRef, Vector3 position, Vector3 forward)
    {
        // Check available prefabs for one whose name matches
        foreach(GameObject prefab in _instance._spawnablePrefabs)
        {
            if(prefab.name == nameRef)
            {
                // Spawn the prefab
                GameObject instance = Instantiate(prefab, position, Quaternion.LookRotation(forward, Vector3.up));

                // Generate a unique id
                long id = DateTime.UtcNow.Ticks;
                while(_spawnedObjectsById.ContainsKey(id))
                    id++;

                // Save the instance and return the id
                _spawnedObjectsById[id] = instance;
                return id;
            }
        }
        Debug.LogError("Tried to spawn Prefab with name: " + nameRef + ", but that prefab has not been set in the GameManager");
        return -1;
    }

    // Remove an object that has previously been spawned
    public static void RemoveSpawnedObject(long id)
    {
        if(_spawnedObjectsById.ContainsKey(id))
        {
            Destroy(_spawnedObjectsById[id]);
            _spawnedObjectsById.Remove(id);
        }
        else
        {
            Debug.LogError("Tried to delete Object with id = " + id + ", but that object has not been spawed by the GameManager");
        }
    }

    // Set the position of an object that has previously been spawned
    public static void SetRotationOfSpawnedObject(long id, Vector3 newPos)
    {
        if(_spawnedObjectsById.ContainsKey(id))
        {
            _spawnedObjectsById[id].transform.position = newPos;
        }
        else
        {
            Debug.LogError("Tried to set pos of Object with id = " + id + ", but that object has not been spawed by the GameManager");
        }
    }

    // Set the rotation of an object that has previously been spawned
    public static void RemoveSpawnedObject(long id, Vector3 newFwd)
    {
        if(_spawnedObjectsById.ContainsKey(id))
        {
            _spawnedObjectsById[id].transform.rotation = Quaternion.LookRotation(newFwd, Vector3.up);
        }
        else
        {
            Debug.LogError("Tried to set rot Object with id = " + id + ", but that object has not been spawed by the GameManager");
        }
    }

}
