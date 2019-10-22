using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private class LaserHitStruct
    {
        public Vector3 Position;
        public Vector3 Normal;
        public GameObject ParticleSystemInstance;
        public bool UpdatedCurrentFrame;

        public LaserHitStruct(Vector3 position, Vector3 normal, GameObject particleSystemInstance)
        {
            Position = position;
            Normal = normal;
            ParticleSystemInstance = particleSystemInstance;
            UpdatedCurrentFrame = true;
        }

        public void SetUpdatedCurrentFrame(bool updatedCurrentFrame)
        {
            UpdatedCurrentFrame = updatedCurrentFrame;
        }

        public bool GetUpdatedCurrentFrame()
        {
            return UpdatedCurrentFrame;
        }
    }

    // All prefabs that can be spawned
    [SerializeField] private GameObject[] _spawnablePrefabs;

    // The particle system that will spawn when a laser hits a surface
    [SerializeField] private GameObject _laserHitParticleSystem;
    [SerializeField] private GameObject _laserEmitterPrefab;

    //The next level to load
    [SerializeField] private int nextLevel;

    // Singleton instance
    private static GameManager _instance;

    private static List<int> _targetIds = new List<int>();      // All Target ids that have been registrered
    private static List<int> _hitTargetIds = new List<int>();   // All Targets that have been hit on the current frame

    // All spawned objects by id reference
    private static Dictionary<long, GameObject> _spawnedObjectsById = new Dictionary<long, GameObject>();

    // All lasers that were split on this frame
    private static Dictionary<Laser, SplitLaserStruct> _splitLasersThisFrame = new Dictionary<Laser, SplitLaserStruct>();

    // A flag for each laser that says if its split lasers should be deleted on this frame or not
    private static Dictionary<Laser, bool> _notifiedLasersThisFrame = new Dictionary<Laser, bool>();

    // All laser hit points that have been registered on this frame
    private static List<LaserHitStruct> _notifiedHitPointsThisFrame = new List<LaserHitStruct>();

    // Reference to the in-scene object manager
    private ObjectManager _objectManager;

    // Has the load next level call been issued?
    private bool _nextLevelLoaded = false;

    // The time at which this scene was started
    private float _sceneStartTime;

    // Has the current level been won?
    private bool _won = false;

    private void Awake()
    {
        _instance = this;
        _objectManager = GetComponent<ObjectManager>();
        // Clear data from last scene
        _splitLasersThisFrame.Clear();
        _notifiedLasersThisFrame.Clear();
        _notifiedHitPointsThisFrame.Clear();
    }

    private void Start()
    {
        _sceneStartTime = Time.time;
        LevelTransitionAnimation.StartAnimateIn();
    }

    private void Update()
    {
        // Check if all Targets have been hit on this frame. If so, we shuold finish the level,
        // but only if we have not previously issued the LoadScene call (avoid issuing multiple calls),
        // and also only if one second has passed (make sure the level has been properly set up)
        if (_hitTargetIds.Count == _targetIds.Count
                && !_nextLevelLoaded
                && Time.time - _sceneStartTime > 1f)
        {
            // Queue won, but wait until all phycons have been removed until moving to the next level
            _won = true;
        }

        // Manual Win cheat, press Q and P at the same time
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.P)
                && !_nextLevelLoaded)
        {
            Win();
        }

        // We have won and removed all phycons. Proceed
        if (_won && _objectManager.CountSpawnedObjects() == 0)
        {
            Win();
        }

        _hitTargetIds.Clear();  // Regardless of if we won or not, clear _hitTargetIds so that it can be rebuilt on the next frame
    }

    private void Win()
    {
        Debug.Log(Time.time - _sceneStartTime);
        Debug.Log("You win!");
        _targetIds.Clear();
        _hitTargetIds.Clear();
        _spawnedObjectsById.Clear();
        _splitLasersThisFrame.Clear();
        _notifiedLasersThisFrame.Clear();
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource> ().clip);
        LevelTransitionAnimation.StartAnimateOut();
        StartCoroutine(LoadNextSceneDelayed());
        _nextLevelLoaded = true;
    }

    public static void DestroyLaserRecursive(Laser laser, bool destroySelf)
    {
        if(_splitLasersThisFrame.ContainsKey(laser))
        {
            DestroyLaserRecursive(_splitLasersThisFrame[laser].SplitLaser1, true);
            DestroyLaserRecursive(_splitLasersThisFrame[laser].SplitLaser2, true);
        }
        // Alexander 11/10 2019: Not entirely sure why the null check on the laser was neccessary here,
        // but it prevents laser references from leaking later
        if(destroySelf && laser != null)
            Destroy(laser.gameObject);
    }

    private void LateUpdate()
    {
        // Temporary lists to avoid the _notifiedLasersThisFrame collection being modified in the loop
        List<Laser> lasersToBeDisabledNextFrame = new List<Laser>();
        List<Laser> lasersToBeDereffed = new List<Laser>();
        List<Laser> splitLasersToBeDestroyed = new List<Laser>();

        // Remove any lasers that have not been updated on this frame
        foreach (Laser laser in _notifiedLasersThisFrame.Keys)
        {
            if(!_notifiedLasersThisFrame[laser])
            {
                // Split lasers should be deleted
                Laser splitLaser1 = _splitLasersThisFrame[laser].SplitLaser1;
                Laser splitLaser2 = _splitLasersThisFrame[laser].SplitLaser2;
                splitLasersToBeDestroyed.Add(splitLaser1);
                splitLasersToBeDestroyed.Add(splitLaser2);
                lasersToBeDereffed.Add(laser);
            }
            else
            {
                // Split lasers have been updated this frame, so don't delete them, only set the Updated flag to false
                lasersToBeDisabledNextFrame.Add(laser);
            }
        }

        foreach(Laser laser in splitLasersToBeDestroyed)
        {
            DestroyLaserRecursive(laser, true);
        }

        // Go over and update the LaerHitStructs that have been stored so far
        List<LaserHitStruct> laserHitStructsToBeDereffed = new List<LaserHitStruct>();
        for( int i=0; i<_notifiedHitPointsThisFrame.Count; i++ )
        {
            LaserHitStruct laserHitStruct = _notifiedHitPointsThisFrame[i];

            // Remove any laser bounce particles that have not been updated on this frame
            // Deref them later
            if(laserHitStruct.GetUpdatedCurrentFrame() == false)
            {
                Destroy(laserHitStruct.ParticleSystemInstance);
                laserHitStructsToBeDereffed.Add(laserHitStruct);
            }
            else
            {
                laserHitStruct.SetUpdatedCurrentFrame(false);
            }
        }

        // Deref and laserHitStructs whose particles where deleted before
        foreach(LaserHitStruct laserHitStruct in laserHitStructsToBeDereffed)
        {
            _notifiedHitPointsThisFrame.Remove(laserHitStruct);
        }

        // Set the Updated flag to false for all lasers
        // Next frame they will be deleted unless they call the GameManager to set their Updated to true first
        foreach (Laser laser in lasersToBeDisabledNextFrame)
        {
            _notifiedLasersThisFrame[laser] = false;
        }

        // Delete the GameManager reference to all lasers that were deleted in the game world
        foreach (Laser laser in lasersToBeDereffed)
        {
            _notifiedLasersThisFrame.Remove(laser);
            _splitLasersThisFrame.Remove(laser);
        }
    }

    private IEnumerator LoadNextSceneDelayed()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(nextLevel);
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

    public static void UnregisterLaser(Laser laser)
    {
        if(_notifiedLasersThisFrame.ContainsKey(laser))
        {
            _notifiedLasersThisFrame.Remove(laser);
        }
        if(_splitLasersThisFrame.ContainsKey(laser))
        {
            _splitLasersThisFrame.Remove(laser);
        }

    }

    // Notify the GameManager that a certain laser has hit a position on this frame
    // The GameManager will proceed with spawning a hit particle system at that point
    public static void NotifyLaserHit(Laser laser, Vector3 position, Vector3 normal)
    {
        // Go through all registered particle system instances for that laser,
        // if one has the same position and normal, update it, and then we are done.
        for( int i=0; i<_notifiedHitPointsThisFrame.Count; i++ )
        {
            LaserHitStruct laserHitStruct = _notifiedHitPointsThisFrame[i];
            if(laserHitStruct.Position == position && laserHitStruct.Normal == normal)
            {
                laserHitStruct.SetUpdatedCurrentFrame(true);
                return;
            }
        }

        GameObject particleSystemInstance = GameObject.Instantiate(_instance._laserHitParticleSystem, position, Quaternion.LookRotation(normal));
        particleSystemInstance.GetComponent<LaserBounceEffect>().SetColor(laser.GetColor());
        _notifiedHitPointsThisFrame.Add(new LaserHitStruct(position, normal, particleSystemInstance));
    }

    // Notify the game manager that a laser has split at the given prism and position.
    // The game manager takes over and spawns 2 new split lasers
    public static void NotifyLaserShouldSplit(Laser laser, Vector3 position, Vector3 forward, Prism rootPrism)
    {
        Laser splitLaser1;
        Laser splitLaser2;

        if(!_splitLasersThisFrame.ContainsKey(laser))
        {
            // Create 2 new split lasers
            Color splitColor1;
            Color splitColor2;
            // Get the split laser colors
            LaserColorDefinitions.GetSplitColors(laser.GetColor(), out splitColor1, out splitColor2);
            splitLaser1 = SpawnLaser(splitColor1, rootPrism);
            splitLaser2 = SpawnLaser(splitColor2, rootPrism);
            _splitLasersThisFrame[laser] = new SplitLaserStruct(splitLaser1, splitLaser2);
        }
        else
        {
            // 2 split lasers from this laser existed on the previous frame,
            // get the reference to them instead of creating them again
            
            // Get split laser references
            splitLaser1 = _splitLasersThisFrame[laser].SplitLaser1;
            splitLaser2 = _splitLasersThisFrame[laser].SplitLaser2;
        }

        splitLaser1.transform.position = position;
        splitLaser2.transform.position = position;

        Vector3 laser1fwd = Quaternion.AngleAxis(45, Vector3.up) * forward;
        Vector3 laser2fwd = Quaternion.AngleAxis(-45, Vector3.up) * forward;
        splitLaser1.GetComponent<Laser>().SetForward(laser1fwd);
        splitLaser2.GetComponent<Laser>().SetForward(laser2fwd);

        // During LateUpdate next frame, don't remove these 2 split lasers
        _notifiedLasersThisFrame[laser] = true;
    }

    // Spawn a laser with the given color and root prism
    // Don't set the start position or rotation, they have to be set later
    private static Laser SpawnLaser(Color color, Prism rootPrism)
    {
        // Laser resource
        Laser laserInstance = GameObject.Instantiate(_instance._laserEmitterPrefab).GetComponent<Laser>();
        laserInstance.SetColor(color);
        laserInstance.SetRootPrism(rootPrism);
        return laserInstance;
    }
    
    // Spawn a prefab at a given location, with a given forward vector
    // nameRef must match the prefab's resource name
    // The unique id is returned, which can be used to move, rotate or destroy the object later
    public static long SpawnPrefab(string nameRef, Vector3 position, Quaternion rotation)
    {
        // Check available prefabs for one whose name matches
        foreach (GameObject prefab in _instance._spawnablePrefabs)
        {
            if (prefab.name == nameRef)
            {
                // Spawn the prefab
                GameObject instance = Instantiate(prefab, position, rotation);

                // Generate a unique id
                long id = DateTime.UtcNow.Ticks;
                while (_spawnedObjectsById.ContainsKey(id))
                    id++;

                // Save the instance and return the id
                _spawnedObjectsById[id] = instance;
                return id;
            }
        }
        Debug.LogError("Tried to spawn Prefab with name: " + nameRef + ", but that prefab has not been set in the GameManager");
        return -1;
    }

    public static long SpawnPrefab(string nameRef, Vector3 position, Vector3 forward)
    {
        return SpawnPrefab(nameRef, position, Quaternion.LookRotation(forward, Vector3.up));
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
    public static void SetRotationOfSpawnedObject(long id, Quaternion newQuat)
    {
        if (_spawnedObjectsById.ContainsKey(id))
        {
            _spawnedObjectsById[id].transform.rotation = newQuat;
        }
        else
        {
            Debug.LogError("Tried to set rot Object with id = " + id + ", but that object has not been spawed by the GameManager");
        }
    }

    public static void SetRotationOfSpawnedObject(long id, Vector3 newFwd)
    {
        SetRotationOfSpawnedObject(id, Quaternion.LookRotation(newFwd, Vector3.up));
    }

    public static GameObject GetSpawnedObject(long id)
    {
        if (!_spawnedObjectsById.ContainsKey(id))
        {
            Debug.LogError($"Tried to access Object with id = {id}, but that object has not been spawned by the GameManager");
        }

        return _spawnedObjectsById[id];
    }

}
