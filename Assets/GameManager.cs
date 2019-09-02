using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _spawnablePrefabs;

    private static GameManager _instance;
    private static List<int> _targetIds = new List<int>();
    private static List<int> _hitTargetIds = new List<int>();
    private static Dictionary<long, GameObject> _spawnedObjectsById = new Dictionary<long, GameObject>();

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (_hitTargetIds.Count == _targetIds.Count)
        {
            Debug.Log("You win!");
        }

        _hitTargetIds.Clear();

        // Cheats, spawn prefabs
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnPrefab("Mirror", new Vector3(0f, 0f, -5f), new Vector3(0f, 0f, 1f));
        }
    }

    public static void RegisterTarget(int id)
    {
        _targetIds.Add(id);
    }

    public static void HitTarget(int id)
    {
        _hitTargetIds.Add(id);
    }
    
    public static long SpawnPrefab(string nameRef, Vector3 position, Vector3 forward)
    {
        foreach(GameObject prefab in _instance._spawnablePrefabs)
        {
            if(prefab.name == nameRef)
            {
                GameObject instance = Instantiate(prefab, position, Quaternion.LookRotation(forward, Vector3.up));
                long id = DateTime.UtcNow.Ticks;
                while(_spawnedObjectsById.ContainsKey(id))
                    id++;
                _spawnedObjectsById[id] = instance;
                return id;
            }
        }
        Debug.LogError("Tried to spawn Prefab with name: " + nameRef + ", but that prefab has not been set in the GameManager");
        return -1;
    }

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

}
