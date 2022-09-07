using System.Collections;
using System.Collections.Generic;
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
}

public enum VibrationType
{
    Pop = 0,
    Peek = 1,
    Nope = 2
}
