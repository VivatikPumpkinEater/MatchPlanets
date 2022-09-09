using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokensTargetController : MonoBehaviour
{
    [SerializeField] private TokensTargetItem _targetItemPrefab = null;
    [SerializeField] private Image _border = null;

    private Dictionary<string, TokensTargetItem> _targetItems = new Dictionary<string, TokensTargetItem>();
    

    public void SetUpTargetsToken(string type, Sprite tokenSprite, int count)
    {
        if (!_border.enabled)
        {
            _border.enabled = true;
        }
        
        if(!_targetItems.ContainsKey(type))
        {
            var targetItem = Instantiate(_targetItemPrefab, transform);
            targetItem.Init(tokenSprite, count);

            _targetItems.Add(type, targetItem);
        }
    }

    public void MinusTargetCount(string type)
    {
        if (_targetItems.ContainsKey(type))
        {
            if (!_targetItems[type].TargetMinus())
            {
                _targetItems.Remove(type);
            }
        }

        if (_targetItems.Count == 0)
        {
           EndGame.Instance.FinishedLvl(FinishType.Win);
        }
    }
}