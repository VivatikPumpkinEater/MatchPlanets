using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool(float duration)
    {
        StartCoroutine(Wait(duration));
    }
    
    private IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);

        ReturnToPool();
    }
}
