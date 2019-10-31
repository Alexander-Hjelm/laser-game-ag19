using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FadeInUi : MonoBehaviour
{
    private Text[] _texts;
    private Image _image;
    private Color[] _targetColors;
    private Color _imageTargetColor;
    private float _elapsedTime;
    private float _startTime;

    public float Duration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_texts != null)
        {
            _elapsedTime = (Time.time - _startTime);
            float t = _elapsedTime / Duration;
            if (t > 1.0)
            {
                return;
            }
            for (int i = 0; i < _texts.Length; i++)
            {
                _texts[i].color = new Color(_targetColors[i].r, _targetColors[i].g, _targetColors[i].b, Mathf.SmoothStep(0, _targetColors[i].a, t));
            }
            _image.color = new Color(_imageTargetColor.r, _imageTargetColor.g, _imageTargetColor.b, Mathf.SmoothStep(0, _imageTargetColor.a, t));
        }
    }

    private void OnEnable()
    {
        _texts = GetComponentsInChildren<Text>();
        _image = GetComponent<Image>();
        _targetColors = _texts.Select(t => t.color).ToArray();
        _imageTargetColor = _image.color;
        foreach (var text in _texts)
        {
            text.color = Color.clear;
        }

        _image.color = Color.clear;
        _elapsedTime = 0f;
        _startTime = Time.time;
    }
}
