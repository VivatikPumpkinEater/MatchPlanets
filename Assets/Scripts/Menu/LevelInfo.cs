using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text _lvlNumber;
    [SerializeField] private TMP_Text _lvlName;
    
    [SerializeField] private Button _startLvl;

    [SerializeField] private GameObject[] _stars;

    [Header("LvlTargets")] [SerializeField]
    private GameObject _pointsTarget;

    [SerializeField] private TMP_Text _pointsTargetTxt;

    [SerializeField] private GameObject _tokensTarget;
    [SerializeField] private TokensTargetItem _tokensTargetPrefab;

    private List<GameObject> _tokensTargets;

    private int _activeLvlNumber;

    private void Awake()
    {
        _tokensTargets = new List<GameObject>();
    }

    private void SetUpLvlInfo(int lvlNumber, LvlData lvlData)
    {
        _lvlNumber.text = lvlNumber.ToString();
        _lvlName.text = lvlData.Name;

        _activeLvlNumber = lvlNumber;

        switch (lvlData.LevelTarget)
        {
            case LevelTarget.Points:
                _tokensTarget.SetActive(false);
                _pointsTarget.SetActive(true);
                _pointsTargetTxt.text = lvlData.PointsTarget.ToString();
                break;
            case LevelTarget.Tokens:
                _pointsTarget.SetActive(false);
                _tokensTarget.SetActive(true);

                ClearTokensTarget();

                foreach (var tokenTarget in lvlData.TokenTargets)
                {
                    var tokenTargetItem = Instantiate(_tokensTargetPrefab, _tokensTarget.transform);
                    tokenTargetItem.Init(tokenTarget.Sprite, tokenTarget.Count, 1f);
                    
                    
                    _tokensTargets.Add(tokenTargetItem.gameObject);
                }

                break;
        }

        _startLvl.onClick.AddListener(() => StartLvl(lvlData));
    }
    
    public void SetUpLvlInfo(int lvlNumber, LvlData lvlData, int starsCount)
    {
        SetUpLvlInfo(lvlNumber, lvlData);
        
        ActivateStars(starsCount);
    }

    private void ActivateStars(int starsCount)
    {
        ClearStars();
        
        for (var i = 0; i < starsCount; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    private void ClearStars()
    {
        for (var i = 0; i < 3; i++)
        {
            _stars[i].SetActive(false);
        }
    }

    private void StartLvl(LvlData lvlData)
    {
        Loading.Instance.Load(lvlData, _activeLvlNumber);
    }

    private void ClearTokensTarget()
    {
        foreach (var target in _tokensTargets)
        {
            Destroy(target);
        }

        _tokensTargets.Clear();
    }
}