using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreTxt = null;

    [SerializeField] private GameObject _pointsTargetPanel = null;
    [SerializeField] private TMP_Text _pointsTargetTxt = null;
    [SerializeField] private Image _falingImage = null;

    [SerializeField] private Image[] _stars = new Image[] { };
    private int _score = 0;

    
    private float _step = 0;
    private float _stepStars = 0;
    private int _currentStar = 0;
    private bool _pointsTargetUse = false;

    private void Awake()
    {
        AddedPoint(0);
    }

    public void InitPointsTarget(int targetPoints)
    {
        _pointsTargetUse = true;
        
        _falingImage.fillAmount = 0;
        _pointsTargetTxt.text = targetPoints.ToString();

        _step = 1f / (targetPoints / 40f);
        
        _pointsTargetPanel.SetActive(true);
    }

    public void InitPerfectScore(int target)
    {
        _stepStars = 1f / ((target / 3f) / 40f);
    }

    public void AddedPoint(int points)
    {
        AudioManager.Instance.GetEffect("Point");
        _score += points;
        _scoreTxt.text = _score.ToString();
        _scoreTxt.gameObject.transform.DOShakeScale(0.5f).OnComplete(()=> _scoreTxt.gameObject.transform.DOScale(Vector3.one, 0.3f));
        
        if (_pointsTargetUse)
        {
            _falingImage.fillAmount += _step;

            if (_falingImage.fillAmount == 1)
            {
                EndGame.Instance.FinishedLvl(FinishType.Win);
            }
            //Debug.Log(_falingImage.fillAmount);
        }

        if(_currentStar < _stars.Length)
        {
            _stars[_currentStar].fillAmount += _stepStars;
            
            if (_stars[_currentStar].fillAmount == 1)
            {
                _stars[_currentStar].transform.DOShakeScale(0.3f);
                
                _currentStar++;
                Loading.Instance.CurrentStars = _currentStar;
            }
        }
    }
}
