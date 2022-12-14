using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bonus
{
    public override List<Vector3> Activate()
    {
        var start = transform.position - (Vector3.right * 7);
        var toDestroy = new List<Vector3>();

        for (int x = 0; x < 14; x++)
        {
            toDestroy.Add(start);
            start += Vector3.right;
        }
        
        return toDestroy;
    }
}
