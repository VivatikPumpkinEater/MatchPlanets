using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokensTargetItem : MonoBehaviour
{
    [SerializeField] private Image _tokenSprite;
    [SerializeField] private Image _filling;
    [SerializeField] private TMP_Text _countTxt;

    [SerializeField] private Image _done;
    

    private int _count;
    private float _step;
    
    public void Init(Sprite tokenSprite, int count)
    {
        _tokenSprite.sprite = tokenSprite;
        _filling.sprite = tokenSprite;
        _filling.fillAmount = 0;

        _count = count;

        _step = 1f / _count;
        
        _done.gameObject.SetActive(false);
        UpdateUI();
    }
    
    public void Init(Sprite tokenSprite, int count, float filling)
    {
        _tokenSprite.sprite = tokenSprite;
        _filling.sprite = tokenSprite;
        _filling.fillAmount = filling;

        _count = count;

        _step = 1f / _count;
        
        _done.gameObject.SetActive(false);
        UpdateUI();
    }

    public bool TargetMinus()
    {
        _count--;

        if (_count > 0)
        {
            _filling.fillAmount += _step;
            UpdateUI();
            
            return true;
        }
        else if (!_done.gameObject.activeSelf)
        {
            _done.gameObject.SetActive(true);
            _done.gameObject.transform.DOShakeScale(0.5f);
            
            _countTxt.gameObject.SetActive(false);
        }

        return false;
    }

    private void UpdateUI()
    {
        _countTxt.text = _count.ToString();
    }
}
