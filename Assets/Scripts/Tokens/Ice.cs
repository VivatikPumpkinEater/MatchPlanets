using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : Token
{
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private Sprite[] _lvls = new Sprite[] { };

    [SerializeField] private int _hp = 1;

    protected override void Start()
    {
        base.Start();
        
        _spriteRenderer.sprite = _lvls[_hp - 1];
    }
    
    public override void Init()
    {
        _hp = Hp;
        _spriteRenderer.sprite = _lvls[_hp - 1];
    }

    protected override void OnMouseDown()
    {
        return;
    }

    protected override void OnMouseEnter()
    {
        return;
    }

    protected override void OnMouseUp()
    {
        return;
    }

    public override bool Destroy()
    {
        _hp--;
        
        if (_hp > 0)
        {
            _spriteRenderer.sprite = _lvls[_hp - 1];
            return false;
        }

        return true;
    }
}
