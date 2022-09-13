using UnityEngine;

public class LvlFill : MonoBehaviour
{
    [SerializeField] private LvlsConstruct _lvlsConstruct;
    [SerializeField] private LvlItemUI _lvlItem;

    [SerializeField] private LevelInfo _levelInfo;

    [SerializeField] private Transform[] _lvlPosition;

    private void Start()
    {
        for (var i = 0; i < _lvlPosition.Length; i++)
        {
            if (_lvlsConstruct.LvlsData.Count != i)
            {
                var lvlItem = Instantiate(_lvlItem, _lvlPosition[i]);
                lvlItem.Init(i + 1, _lvlsConstruct.LvlsData[i], _levelInfo);

                if (_lvlsConstruct.LvlsData[i].LevelPassed)
                {
                    lvlItem.ActivateStar(_lvlsConstruct.LvlsData[i].Stars);
                }

                lvlItem.ActivateStar(0);

                if (_lvlsConstruct.LvlsData[i].LevelPassed && i + 1 < _lvlsConstruct.LvlsData.Count &&
                    !_lvlsConstruct.LvlsData[i + 1].LevelUnlock)
                {
                    var lvlData = _lvlsConstruct.LvlsData[i + 1];
                    lvlData.LevelUnlock = true;

                    _lvlsConstruct.LvlsData[i + 1] = lvlData;
                }
            }
            else
            {
                break;
            }
        }
    }
}