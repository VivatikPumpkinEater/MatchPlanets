using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevOptions : MonoBehaviour
{
    [SerializeField] private Button _unlockAllLevels;
    [SerializeField] private Button _resetAllLevels;

    [SerializeField] private LvlsConstruct _lvlsConstruct;
    
    private void Start()
    {
        _unlockAllLevels.onClick.AddListener(UnlockLevels);
        _resetAllLevels.onClick.AddListener(ResetLevels);
    }

    private void UnlockLevels()
    {
        var lvlDatas = new List<LvlData>();
        
        foreach (var lvlData in _lvlsConstruct.LvlsData)
        {
            var data = lvlData;
            data.LevelUnlock = true;
            
            lvlDatas.Add(data);
        }

        for (int i = 0; i < lvlDatas.Count; i++)
        {
            _lvlsConstruct.LvlsData[i] = lvlDatas[i];
        }

        SceneManager.LoadScene(0);
    }

    private void ResetLevels()
    {
        var lvlDatas = new List<LvlData>();
        
        foreach (var lvlData in _lvlsConstruct.LvlsData)
        {
            var data = lvlData;
            data.LevelUnlock = false;
            data.LevelPassed = false;

            data.Stars = 0;

            lvlDatas.Add(data);
        }

        for (int i = 0; i < lvlDatas.Count; i++)
        {
            if (i != 0)
            {
                _lvlsConstruct.LvlsData[i] = lvlDatas[i];
            }
            else
            {
                var lvlData = lvlDatas[i];
                lvlData.LevelUnlock = true;
                
                _lvlsConstruct.LvlsData[i] = lvlData;
            }
            
        }
        
        SceneManager.LoadScene(0);
    }
}
