using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _lvlNumber = null;

    [SerializeField] private GameObject[] _stars = new GameObject[] { };

    [SerializeField] private Button _button = null;

    [SerializeField] private Animator _animator = null;

    private LvlData _lvlData = new LvlData();

    private LevelInfo _levelInfo = null;
    private int _lvlNum;
    private void Awake()
    {
        foreach (var star in _stars)
        {
            star.SetActive(false);
        }
        
        _button.onClick.AddListener(LoadLvl);
    }

    public void Init(int lvlNumber, LvlData lvlData, LevelInfo lvlInfo)
    {
        _levelInfo = lvlInfo;
        _lvlNum = lvlNumber;
        
        _lvlNumber.text = lvlNumber.ToString();
        _lvlData = lvlData;
        
        _button.interactable = lvlData.LevelUnlock;
        _animator.enabled = lvlData.LevelUnlock;

        if (!_button.interactable)
        {
            _button.transition = Selectable.Transition.ColorTint;
        }
    }

    public void ActivateStar(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    private void LoadLvl()
    {
        _levelInfo.gameObject.SetActive(true);
        _levelInfo.transform.localScale = Vector3.zero;
        
        _levelInfo.SetUpLvlInfo(_lvlNum, _lvlData, _lvlData.Stars);
        

        _levelInfo.transform.DOScale(Vector3.one, 0.3f);
    }
}