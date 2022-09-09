using UnityEngine;

public abstract class Window : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    public bool FullScreen { get; set; } = true;
    public Window CurrentWindow { get; protected set; } = null;

    public delegate void OpenEventHandler(Window sender);

    public event OpenEventHandler OnOpen;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        UIManager.AddedWindow(this);
    }

    public void Open()
    {
        IsOpen = true;
        if (OnOpen != null)
            OnOpen(this);

        SelfOpen();
    }

    protected abstract void SelfOpen();

    public void Close()
    {
        IsOpen = false;
        
        SelfClose();
    }

    protected abstract void SelfClose();

    protected void ChangeCurrentWindow(Window sender)
    {
        if (CurrentWindow != null)
            CurrentWindow.Close();

        CurrentWindow = sender;
    }
}