using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private struct SplitLaserStruct
    {
        public Laser SplitLaser1;
        public Laser SplitLaser2;

        public SplitLaserStruct(Laser splitLaser1, Laser splitLaser2)
        {
            SplitLaser1 = splitLaser1;
            SplitLaser2 = splitLaser2;
        }
    }

    // All prefabs that can be spawned
    [SerializeField] private GameObject[] _spawnablePrefabs;

    // Singleton instance
    private static GameManager _instance;

    private static List<int> _targetIds = new List<int>();      // All Target ids that have been registrered
    private static List<int> _hitTargetIds = new List<int>();   // All Targets that have been hit on the current frame

    // All spawned objects by id reference
    private static Dictionary<long, GameObject> _spawnedObjectsById = new Dictionary<long, GameObject>();

    private static Dictionary<Laser, SplitLaserStruct> _splitLasersThisFrame = new Dictionary<Laser, SplitLaserStruct>();

    private static Dictionary<Laser, bool> _notifiedLasersThisFrame = new Dictionary<Laser, bool>();

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

    private void LateUpdate()
    {
        // TODO
        // Remove any lasers that have not been updated on this frame
        foreach (Laser laser in _notifiedLasersThisFrame.Keys)
        {
            if(!_notifiedLasersThisFrame[laser])
            {
                // Split lasers should be deleted
                Laser splitLaser1 = _splitLasersThisFrame[laser].SplitLaser1;
                Laser splitLaser2 = _splitLasersThisFrame[laser].SplitLaser2;
                Destroy(splitLaser1);
                Destroy(splitLaser2);
                _splitLasersThisFrame.Remove(laser);
            }
            else
            {
                // Split lasers have been updated this frame, only set the Updated flag to false
                _notifiedLasersThisFrame[laser] = false;
            }
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

    public static void NotifyLaserShouldSplit(Laser laser, Vector3 position, Vector3 forward)
    {
        Laser splitLaser1;
        Laser splitLaser2;

        if(!_splitLasersThisFrame.ContainsKey(laser))
        {
            // Split the laser
            Color splitColor1;
            Color splitColor2;
            LaserColorDefinitions.GetSplitColors(laser.GetColor(), out splitColor1, out splitColor2);
            splitLaser1 = SpawnLaser(splitColor1);
            splitLaser2 = SpawnLaser(splitColor2);
            _splitLasersThisFrame[laser] = new SplitLaserStruct(splitLaser1, splitLaser2);
        }
        else
        {
            // Get split laser references
            splitLaser1 = _splitLasersThisFrame[laser].SplitLaser1;
            splitLaser2 = _splitLasersThisFrame[laser].SplitLaser2;
        }

        splitLaser1.transform.position = position;
        splitLaser2.transform.position = position;

        Vector3 laser1fwd = Quaternion.AngleAxis(45, Vector3.up) * forward;
        Vector3 laser2fwd = Quaternion.AngleAxis(-45, Vector3.up) * forward;

        splitLaser1.transform.rotation = Quaternion.LookRotation(laser1fwd);
        splitLaser2.transform.rotation = Quaternion.LookRotation(laser2fwd);

        _notifiedLasersThisFrame[laser] = true;
    }

    private static Laser SpawnLaser(Color color)
    {
        GameObject laserPrefab = Resources.Load<GameObject>("Prefabs/LaserStartPosition");
        GameObject laserInstance = GameObject.Instantiate(laserPrefab);
        laserInstance.GetComponent<Laser>().SetColor(color);
        return laserInstance.GetComponent<Laser>();
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
    public static void SetPositionOfSpawnedObject(long id, Vector3 newPos)
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
    public static void SetRotationOfSpawnedObject(long id, Vector3 newFwd)
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
