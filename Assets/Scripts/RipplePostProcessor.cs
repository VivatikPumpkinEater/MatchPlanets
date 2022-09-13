using UnityEngine;

public class RipplePostProcessor : MonoBehaviour
{
    [SerializeField] private Material _rippleMaterial;
    [SerializeField] private float _maxAmount = 50f;
 
    [Range(0,1)] [SerializeField]
    private float _friction;
 
    private float _amount;
 
    void Update()
    {
        _rippleMaterial.SetFloat("_Amount", _amount);
        _amount *= _friction;
    }

    public void RippleEffect(Vector2 position)
    {
        _amount = _maxAmount;
        _rippleMaterial.SetFloat("_CenterX", position.x);
        _rippleMaterial.SetFloat("_CenterY", position.y);
    }
 
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, _rippleMaterial);
    }
}
