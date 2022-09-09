using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance = null;

    public static Window CurrentWindow;
    public Window LastWindow;

    public Window MicroWindow;

    private List<Window> _windows = new List<Window>();

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    public static void AddedWindow(Window window)
    {
        _instance._windows.Add(window);
    }
    
    public static void Open<T>() where T : Window
    {
        _instance.Search<T>();
    }

    private void Search<T>() where T : Window
    {
        foreach (var window in _windows)
        {
            if (window is T)
            {
                window.Open();
                if (window.FullScreen)
                {
                    LastWindow = CurrentWindow;
                    CurrentWindow = window;
                    LastWindow.Close();

                    if (MicroWindow)
                    {
                        MicroWindow.Close();
                        MicroWindow = null;
                    }
                }
                else
                {
                    if (MicroWindow && !MicroWindow.Equals(window))
                    {
                        MicroWindow.Close();
                    }

                    MicroWindow = window;
                }

                break;
            }
        }
    }
}