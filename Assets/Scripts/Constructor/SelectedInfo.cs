using UnityEngine;
using UnityEngine.UI;

public class SelectedInfo : MonoBehaviour
{
    [SerializeField] private Button _cellReset;
    [SerializeField] private Image _inputStatus;
    [SerializeField] private Image _currentObject;

    public Button CellReset => _cellReset;
    public Image InputStatus => _inputStatus;
    public Image CurrentObject => _currentObject;
}
