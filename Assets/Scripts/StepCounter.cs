using DG.Tweening;
using TMPro;
using UnityEngine;

public class StepCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _stepTxt = null;

    [SerializeField] private LoadLvl _lvl = null;

    [SerializeField] private EndGame _endGame = null;
    
    public int StepLeft { get; private set; } = 0;

    private void Start()
    {
        _lvl.StepsLoadedEvent += Init;
        LineController.Instance.EndStep += EndStep;
        _endGame.AddSteps += AddStep;
    }

    private void Init(int steps)
    {
        StepLeft = steps;
        UpdateUI();
    }

    private void AddStep(int stepCount)
    {
        _endGame.AddSteps -= AddStep;
        
        StepLeft += stepCount;
        _stepTxt.gameObject.transform.DOShakeScale(0.2f);
        UpdateUI();
    }

    private void EndStep()
    {
        StepLeft--;

        if (StepLeft <= 0)
        {
            EndGame.Instance.FinishedLvl(FinishType.Lose);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        _stepTxt.text = StepLeft.ToString();
    }
}