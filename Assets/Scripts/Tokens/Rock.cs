using UnityEngine;

public class Rock : Token
{
    [SerializeField] private Sprite[] _lvls;

    [SerializeField] private int _hp = 1;

    private void Awake()
    {
        Type = TokenType.Rock;
    }
    
    protected override void Start()
    {
        base.Start();
        
        if(Hp < _hp)
        {
            Hp = _hp;
        }
        
        SpriteRenderer.sprite = _lvls[_hp - 1];
    }

    public override void Init()
    {
        _hp = Hp;
        SpriteRenderer.sprite = _lvls[_hp - 1];
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
            SpriteRenderer.sprite = _lvls[_hp - 1];
            return false;
        }

        return true;
    }
}