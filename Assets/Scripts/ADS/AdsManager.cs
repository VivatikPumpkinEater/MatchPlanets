using System;
using System.Collections.Generic;
using AppodealAds.Unity.Common;
using AppodealAds.Unity.Api;
using UnityEngine;

public class AdsManager : MonoBehaviour, IAppodealInitializationListener, IRewardedVideoAdListener
{
    public static System.Action<RewardVideoStatus> RewardVideoEndEvent;

    private static AdsManager _instance = null;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        int adTypes = Appodeal.REWARDED_VIDEO;
        string appKey = "cfc0450e92bdfbcf1f93c0978d09c4eb1987a22d7f47747f";
        Appodeal.initialize(appKey, adTypes);
        
        Appodeal.setUseSafeArea(true);
        Appodeal.setRewardedVideoCallbacks(this);
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

    public void onRewardedVideoLoaded(bool isPrecache)
    {
        Debug.Log("onRewardedVideoLoaded");
        Debug.Log($"getPredictedEcpm(): {Appodeal.getPredictedEcpm(Appodeal.REWARDED_VIDEO)}");
    }

    public void onRewardedVideoFailedToLoad()
    {
        Debug.Log("onRewardedVideoFailedToLoad");
    }

    public void onRewardedVideoShowFailed()
    {
        Debug.Log("onRewardedVideoShowFailed");
    }

    public void onRewardedVideoShown()
    {
        Debug.Log("onRewardedVideoShown");
    }

    public void onRewardedVideoClosed(bool finished)
    {
        Debug.Log($"onRewardedVideoClosed. Finished - {finished}");

        if (!finished)
        {
            RewardVideoEndEvent?.Invoke(RewardVideoStatus.Cancel);
        }
    }

    public void onRewardedVideoFinished(double amount, string name)
    {
        Debug.Log("onRewardedVideoFinished. Reward: " + amount + " " + name);
        
        RewardVideoEndEvent?.Invoke(RewardVideoStatus.Finished);
    }

    public void onRewardedVideoExpired()
    {
        Debug.Log("onRewardedVideoExpired");
    }

    public void onRewardedVideoClicked()
    {
        Debug.Log("onRewardedVideoClicked");
    }
}

public enum RewardVideoStatus
{
    Finished,
    Cancel
}
