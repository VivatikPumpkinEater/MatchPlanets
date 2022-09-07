using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamResize : MonoBehaviour
{
    [SerializeField] private Vector2 _refResolution;
    
    private void Awake()
    {
        Application.targetFrameRate = 120;
    }
    
    private void Start()
    {
        float refAspect = _refResolution.x / _refResolution.y;
        float scaleMultiplier = refAspect / Camera.main.aspect;
        float newSize = Camera.main.orthographicSize * scaleMultiplier;

        Camera.main.orthographicSize = newSize;
    }
}
