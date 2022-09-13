using UnityEngine;

public class ConstructCell : MonoBehaviour
{
    private FieldConstructor _fieldConstructor;

    public CellInfo Cell { get; private set; }
    public GameObject SpawnPoint { get; private set; }

    private void Awake()
    {
        _fieldConstructor = GetComponentInParent<FieldConstructor>();
    }

    private void OnMouseDown()
    {
        if(FSM.Status == GameStatus.Game)
        {
            if (Cell == null && !_fieldConstructor.Delete && !_fieldConstructor.SpawnPoint)
            {
                var cell = Instantiate(_fieldConstructor.Cell, transform);
                Cell = cell;
            }
            else if (Cell != null && _fieldConstructor.ActiveObject != null && !_fieldConstructor.Delete)
            {
                if (Cell.ActualToken != null)
                {
                    Destroy(Cell.ActualToken.gameObject);
                    Cell.ActualToken = null;
                }

                var token = Instantiate(_fieldConstructor.ActiveObject, transform);
                Cell.ActualToken = token;
            }
            else if (Cell == null && _fieldConstructor.SpawnPoint != null && !_fieldConstructor.Delete)
            {
                var spawnPoint = Instantiate(_fieldConstructor.SpawnPoint, transform);
                SpawnPoint = spawnPoint;
            }
            else if (_fieldConstructor.Delete)
            {
                ClearCell();
            }
        }
    }

    public void ClearCell()
    {
        if (Cell)
        {
            if (Cell.ActualToken) Destroy(Cell.ActualToken.gameObject);
            Destroy(Cell.gameObject);
            Cell = null;
        }

        if (SpawnPoint)
        {
            Destroy(SpawnPoint);
            SpawnPoint = null;
        }
    }
}