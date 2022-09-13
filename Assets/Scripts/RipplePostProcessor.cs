using UnityEngine;

public class RipplePostProcessor : MonoBehaviour
{
    [SerializeField] private Material _rippleMaterial;
    [SerializeField] private float _maxAmount = 50f;
 
    [Range(0,1)] [SerializeField]
    private float _friction;
 
    private float _amount;
    private static readonly int CenterY = Shader.PropertyToID("_CenterY");
    private static readonly int CenterX = Shader.PropertyToID("_CenterX");
    private static readonly int Amount = Shader.PropertyToID("_Amount");

    void Update()
    {
        _rippleMaterial.SetFloat(Amount, _amount);
        _amount *= _friction;
    }

    public void RippleEffect(Vector2 position)
    {
        _amount = _maxAmount;
        _rippleMaterial.SetFloat(CenterX, position.x);
        _rippleMaterial.SetFloat(CenterY, position.y);
    }
 
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, _rippleMaterial);
    }
}
