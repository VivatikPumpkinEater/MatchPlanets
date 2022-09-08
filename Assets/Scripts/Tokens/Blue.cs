
using System;

public class Blue : Token
{
    private void Awake()
    {
        Type = TokenType.Blue;
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
    }

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
    }
}
