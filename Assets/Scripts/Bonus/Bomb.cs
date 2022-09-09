using System.Collections.Generic;
using UnityEngine;

public class Bomb : Bonus
{
    private const int RADIUS = 5;
    
    public override List<Vector3> Activate()
    {
        var start = transform.position - (Vector3.right * 2) - (Vector3.up * 2);

        var toDestroy = new List<Vector3>();

        for (int y = 0; y < RADIUS; y++)
        {
            for (int x = 0; x < RADIUS; x++)
            {
                toDestroy.Add(start);
                start += Vector3.right;
            }
            
            start += (Vector3.left * RADIUS) + Vector3.up;
        }

        return toDestroy;
    }
}
