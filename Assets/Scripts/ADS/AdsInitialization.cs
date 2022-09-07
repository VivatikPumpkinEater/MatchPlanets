using System.Collections.Generic;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdsInitialization : MonoBehaviour, IAppodealInitializationListener
{
    private void Start()
    {
        int adTypes = Appodeal.REWARDED_VIDEO;
        string appKey = "cfc0450e92bdfbcf1f93c0978d09c4eb1987a22d7f47747f";
        Appodeal.initialize(appKey, adTypes);
        
        Appodeal.setUseSafeArea(true);
        
        if (EndGame.Instance)
        {
            EndGame.Instance.InitAdsCallback();
        }
    }

    public void onInitializationFinished(List<string> errors)
    {
        string output = errors == null ? string.Empty : string.Join(", ", errors);
        Debug.Log($"onInitializationFinished(errors:[{output}])");
        
        Debug.Log($"getRewardParameters(): {Appodeal.getRewardParameters()}");
        Debug.Log($"getNativeSDKVersion(): {Appodeal.getNativeSDKVersion()}");

        var networksList = Appodeal.getNetworks(Appodeal.REWARDED_VIDEO);
        output = networksList == null ? string.Empty : string.Join(", ", (networksList.ToArray()));
        Debug.Log($"getNetworks() for RV: {output}");
    }
}