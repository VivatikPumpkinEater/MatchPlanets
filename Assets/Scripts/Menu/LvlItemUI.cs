using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _lvlNumber;

    [SerializeField] private GameObject[] _stars;

    [SerializeField] private Button _button;

    [SerializeField] private Animator _animator;

    private LvlData _lvlData;
    private LevelInfo _levelInfo;
    private int _lvlNum;
    
    private void Awake()
    {
        foreach (var star in _stars)
        {
            star.SetActive(false);
        }

        _lvlData = new LvlData();
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
        for (var i = 0; i < count; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    private void LoadLvl()
    {
        UIManager.Open<LevelInfoWindow>();
        
        _levelInfo.SetUpLvlInfo(_lvlNum, _lvlData, _lvlData.Stars);
    }
}