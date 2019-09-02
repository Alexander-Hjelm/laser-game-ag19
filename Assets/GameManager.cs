using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private static List<int> _targetIds = new List<int>();
    private static List<int> _hitTargetIds = new List<int>();

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
    }

    public static void RegisterTarget(int id)
    {
        _targetIds.Add(id);
    }

    public static void HitTarget(int id)
    {
        _hitTargetIds.Add(id);
    }
}
