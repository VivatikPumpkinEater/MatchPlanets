using UnityEngine;
using UnityEngine.UI;

public class SelectedInfo : MonoBehaviour
{
    [SerializeField] private Button _cellReset = null;
    [SerializeField] private Image _inputStatus = null;
    [SerializeField] private Image _currentObject = null;

    public Button CellReset => _cellReset;
    public Image InputStatus => _inputStatus;
    public Image CurrentObject => _currentObject;
}
