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
        var refAspect = _refResolution.x / _refResolution.y;
        
        var cam = Camera.main;
        
        if (cam != null)
        {
            var scaleMultiplier = refAspect / cam.aspect;
            var newSize = cam.orthographicSize * scaleMultiplier;

            cam.orthographicSize = newSize;
        }
    }
}
