using UnityEngine;

public abstract class Window : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    public bool FullScreen { get; set; } = true;

    protected virtual void Start()
    {
        UIManager.AddedWindow(this);
    }

    public void Open()
    {
        IsOpen = true;

        SelfOpen();
    }

    protected abstract void SelfOpen();

    public void Close()
    {
        IsOpen = false;
        
        SelfClose();
    }

    protected abstract void SelfClose();
}