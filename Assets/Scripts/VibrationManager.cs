using System;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance = null;

    public bool Vibration { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        global::Vibration.Init();
        InitVibro();
    }

    private void InitVibro()
    {
        if (PlayerPrefs.HasKey("VibroStatus"))
        {
            Vibration = Boolean.Parse(PlayerPrefs.GetString("VibroStatus"));
        }
    }

    public void VibrationAccess()
    {
        Vibration = !Vibration;

        if (Vibration)
        {
            GetVibration(VibrationType.Peek);
        }
    }

    public void GetVibration(VibrationType vibrationType)
    {
        if (Vibration)
        {
            switch (vibrationType)
            {
                case VibrationType.Pop:
                    global::Vibration.VibratePop();
                    break;
                case VibrationType.Peek:
                    global::Vibration.VibratePeek();
                    break;
                case VibrationType.Nope:
                    global::Vibration.VibrateNope();
                    break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("VibroStatus", Vibration.ToString());
    }
}

public enum VibrationType
{
    Pop = 0,
    Peek = 1,
    Nope = 2
}
