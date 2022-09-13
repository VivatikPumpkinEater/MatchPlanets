using System;
using UnityEngine;

[Serializable]
public class Token : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer => _spriteRenderer = _spriteRenderer ? _spriteRenderer : GetComponent<SpriteRenderer>();
    public Bonus Bonus { get; set; } = null;
    public int Hp { get; set; } = 1;
    public TokenType Type;
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
        if(_lineController != null && FSM.Status == GameStatus.Game)
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
public enum TokenType
{
        Red,
        Blue,
        Green,
        Pink,
        Yellow,
        Rock,
        Ice,
        Null
}