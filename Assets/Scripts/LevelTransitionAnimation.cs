using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionAnimation : MonoBehaviour
{
    [SerializeField] private float _transitionStep = 0.01f;

    private static LevelTransitionAnimation _instance;
    private Material material;

    private void Awake()
    {
        _instance = this;
        material = GetComponent<Renderer>().material;
    }
    
    public static void StartAnimateIn()
    {
        _instance.StartCoroutine(_instance.AnimateIn());
    }

    public static void StartAnimateOut()
    {
        _instance.StartCoroutine(_instance.AnimateOut());
    }

    private IEnumerator AnimateIn()
    {
        material.SetFloat("_CutoffController", 0.0f);
        float cutoff = 0.0f;
        while(cutoff < 1.0f)
        {
            cutoff += _transitionStep;
            material.SetFloat("_CutoffController", cutoff);
            yield return null;
        }
        material.SetFloat("_CutoffController", 1.0f);
    }

    private IEnumerator AnimateOut()
    {
        material.SetFloat("_CutoffController", 1.0f);
        float cutoff = 1.0f;
        while(cutoff > 0.0f)
        {
            cutoff -= _transitionStep;
            material.SetFloat("_CutoffController", cutoff);
            yield return null;
        }
        material.SetFloat("_CutoffController", 0.0f);
    }
}
