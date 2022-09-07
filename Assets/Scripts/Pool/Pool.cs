using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private PoolObject _prefab = null;
    [SerializeField] private Transform _container = null;
    [SerializeField] private int _startCopacity = 1;

    private List<PoolObject> _pool = new List<PoolObject>();

    private void Awake()
    {
        _pool.Clear();

        CreatePool();
    }

    private void CreatePool()
    {
        _pool = new List<PoolObject>();

        for (int i = 0; i < _startCopacity; i++)
        {
            CreateElement();
        }
    }

    private PoolObject CreateElement()
    {
        var createObject = Instantiate(_prefab, _container);
        createObject.gameObject.SetActive(false);

        _pool.Add(createObject);

        return createObject; // ?
    }

    private void TryGetElement(out PoolObject element)
    {
        foreach (var i in _pool)
        {
            if (!i.gameObject.activeInHierarchy)
            {
                element = i;
                i.gameObject.SetActive(true);
                return;
            }
        }

        element = CreateElement();
        element.gameObject.SetActive(true);
    }

    public PoolObject GetFreeElement() // 
    {
        TryGetElement(out var element);

        return element;
    }

    public PoolObject GetFreeElement(Vector3 position)
    {
        var element = GetFreeElement();
        element.transform.position = position;
        return element;
    }

    public PoolObject GetFreeElement(Transform transform)
    {
        var element = GetFreeElement();
        element.transform.position = transform.position;
        element.transform.rotation = transform.rotation;
        return element;
    }

    public PoolObject GetFreeElement(Vector3 position, Quaternion rotation)
    {
        var element = GetFreeElement(position);
        element.transform.rotation = rotation;
        return element;
    }
}
