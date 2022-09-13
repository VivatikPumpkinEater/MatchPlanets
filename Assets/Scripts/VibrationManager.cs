using System;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    private static VibrationManager _instance;

    private bool _vibration = true;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        Vibration.Init();
        InitVibro();
    }

    private void InitVibro()
    {
        if (PlayerPrefs.HasKey("VibroStatus"))
        {
            _vibration = Boolean.Parse(PlayerPrefs.GetString("VibroStatus"));
        }
    }

    public static bool GetVibrationStatus()
    {
        return _instance._vibration;
    }

    public static void ChangeVibrationStatus()
    {
        _instance.VibrationAccess();
    }
    
    public static void GetVibration(VibrationType vibrationType)
    {
        _instance.SearchVibration(vibrationType);
    }
    
    private void VibrationAccess()
    {
        _vibration = !_vibration;

        if (_vibration)
        {
            GetVibration(VibrationType.Peek);
        }
    }

    private void SearchVibration(VibrationType vibrationType)
    {
        if (_vibration)
        {
            switch (vibrationType)
            {
                case VibrationType.Pop:
                    Vibration.VibratePop();
                    break;
                case VibrationType.Peek:
                    Vibration.VibratePeek();
                    break;
                case VibrationType.Nope:
                    Vibration.VibrateNope();
                    break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("VibroStatus", _vibration.ToString());
    }
}

public enum VibrationType
{
    Pop = 0,
    Peek = 1,
    Nope = 2
}
