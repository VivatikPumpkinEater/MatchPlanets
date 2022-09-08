using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private CellInfo _cellPrefabTest = null;
    
    public System.Action<Dictionary<Vector3, CellInfo>, List<Vector3>> LvlField;

    private const int MAX_HEIGHT = 11;
    private const int MAX_WIDTH = 7;
    

    private Dictionary<Vector3, CellInfo> _field = new Dictionary<Vector3, CellInfo>();
    private List<Vector3> _spawnPoint = new List<Vector3>();

    private void Start()
    {
        GeneratedField();
    }

    private void GeneratedField()
    {
        if (_field.Count > 0)
        {
            RemoveField();
        }

        for (int y = 0; y < MAX_HEIGHT; y++)
        {
            for (int x = 0; x < MAX_WIDTH; x++)
            {
                var cell = Instantiate(_cellPrefabTest, transform);
                cell.transform.position = new Vector3(x, y);
                
                _field.Add(cell.transform.position, cell);

                if (y == MAX_HEIGHT - 1)
                {
                    _spawnPoint.Add(new Vector3(x, y + 1));
                }
            }
        }

        LvlField?.Invoke(_field, _spawnPoint);
    }

    private void RemoveField()
    {
        foreach (var cell in _field.Keys)
        {
            Destroy(_field[cell].gameObject);
        }

        _field.Clear();
    }
}