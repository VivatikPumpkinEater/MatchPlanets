using DG.Tweening;
using TMPro;
using UnityEngine;

public class StepCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _stepTxt;

    [SerializeField] private GameController _gameController;

    [SerializeField] private EndGame _endGame;
    
    private int _stepLeft;

    private void Start()
    {
        _gameController.StepsLoadedEvent += Init;
        LineController.Instance.TurnCompletedEvent += EndStep;
        _endGame.AddedStepsEvent += AddStep;
    }

    private void Init(int steps)
    {
        _stepLeft = steps;
        UpdateUI();
    }

    private void AddStep(int stepCount)
    {
        _endGame.AddedStepsEvent -= AddStep;
        
        _stepLeft += stepCount;
        _stepTxt.gameObject.transform.DOShakeScale(0.2f);
        UpdateUI();
    }

    private void EndStep()
    {
        _stepLeft--;

        if (_stepLeft <= 0)
        {
            EndGame.Instance.FinishedLvl(FinishType.Lose);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        _stepTxt.text = _stepLeft.ToString();
    }
}