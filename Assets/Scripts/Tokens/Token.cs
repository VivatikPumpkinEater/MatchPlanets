using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[Serializable]
public class Token : MonoBehaviour
{
    [SerializeField] protected GameObject _outline = null;

    //public const int POINTS = 40;
    public SpriteRenderer SpriteRenderer => _spriteRenderer = _spriteRenderer ?? GetComponent<SpriteRenderer>();
    public Bonus Bonus { get; set; } = null;
    public int Hp { get; set; } = 1;
    public virtual bool Moving { get; set; } = false;

    private SpriteRenderer _spriteRenderer = null;
    
    private LineController _lineController = null;

    protected virtual void Start()
    {
        _lineController = LineController.Instance;
        
    }
    
    public virtual void Init()
    {
        return;
    }

    protected virtual void OnMouseDown()
    {
        if(_lineController != null)
        {
            _lineController.AddPosition(transform.position, this);
        }
    }

    protected virtual void OnMouseEnter()
    {
        if (_lineController != null && _lineController.InProgress)
        {
            _lineController.AddPosition(transform.position, this);
        }
    }

    protected virtual void OnMouseUp()
    {
        if(_lineController != null)
        {
            _lineController.ClearLine();
        }
    }

    public virtual bool Destroy()
    {
        return true;
    }
}