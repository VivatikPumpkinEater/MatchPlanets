using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldConstructor : MonoBehaviour
{
    [SerializeField] private ConstructCell _constructCellPrefab = null;
    [SerializeField] private CellInfo _cellPrefab = null;

    [SerializeField] private ScrollController _scrollController = null;
    [SerializeField] private SelectedInfo _selectedInfo = null;

    [SerializeField] private Sprite[] _inputStatus = new Sprite[2];

    [SerializeField] private Button _reset = null;
    [SerializeField] private Button _save = null;

    [SerializeField] private LvlsConstruct _lvlsConstructData = null;

    [Header("Save Panel")] [SerializeField]
    private GameObject _savePanel = null;

    [SerializeField] private GameObject _errorScreen = null;
    [SerializeField] private TMP_Text _errorTxt = null;

    [SerializeField] private TMP_InputField _inputName = null;

    [SerializeField] private TMP_InputField _stepCount = null;
    [SerializeField] private TMP_InputField _scoreForStars = null;

    [SerializeField] private ToggleGroup _levelTargetsToggles = null;

    [Space(15)] [SerializeField] private Toggle _pointsTarget = null;
    [SerializeField] private GameObject _pointsTargetInputScreen = null;
    [SerializeField] private TMP_InputField _pointsTargetInputField = null;

    [Space(15)]
    [SerializeField] private TokensTargetSettings _tokensTargetInputScreen = null;

    [SerializeField] private Button _cancel = null;
    [SerializeField] private Button _saveLvl = null;

    public Token ActiveObject { get; private set; } = null;
    public GameObject SpawnPoint { get; private set; } = null;

    public bool Delete { get; private set; } = false;
    public CellInfo Cell { get; private set; } = null;

    private List<ConstructCell> _constructCells = new List<ConstructCell>();

    private const int MaxHeight = 12; // layer 12 only for spawnPoint
    private const int MaxWidth = 7;


    private void Awake()
    {
        GeneratedSimpleField();

        _reset.onClick.AddListener(Reset);
        _save.onClick.AddListener(OpenSaveSettings);

        _pointsTarget.onValueChanged.AddListener(ShowTargetsSettings);

        FSM.SetGameStatus(GameStatus.Game);
    }

    private void Start()
    {
        _scrollController.SelectedToken += Selected;
        _selectedInfo.CellReset.onClick.AddListener(WriteDelete);

        InitButtons();
    }

    private void InitButtons()
    {
        _saveLvl.onClick.AddListener(CheckInputData);
        _cancel.onClick.AddListener(CloseSaveSettings);
    }

    private void GeneratedSimpleField()
    {
        for (int y = 0; y < MaxHeight; y++)
        {
            for (int x = 0; x < MaxWidth; x++)
            {
                var cell = Instantiate(_constructCellPrefab, transform);
                cell.transform.position = new Vector3(x, y);

                _constructCells.Add(cell);

                if (y == MaxHeight - 1)
                {
                    cell.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
            }
        }

        Cell = _cellPrefab;
    }

    private void Selected(Token token, Image icon, GameObject spawnPoint)
    {
        if (token != null)
        {
            ActiveObject = token;
            SpawnPoint = null;
        }
        else
        {
            SpawnPoint = spawnPoint;
            ActiveObject = null;
        }

        _selectedInfo.CurrentObject.sprite = icon.sprite;
    }

    private void WriteDelete()
    {
        Delete = !Delete;

        switch (Delete)
        {
            case true:
                _selectedInfo.InputStatus.sprite = _inputStatus[0];
                break;
            case false:
                _selectedInfo.InputStatus.sprite = _inputStatus[1];
                break;
        }
    }

    private void Reset()
    {
        foreach (var constructCell in _constructCells)
        {
            if (constructCell.Cell != null)
            {
                constructCell.ClearCell();
            }
        }
    }

    private void OpenSaveSettings()
    {
        FSM.SetGameStatus(GameStatus.Wait);

        _savePanel.SetActive(true);
    }

    private void CloseSaveSettings()
    {
        FSM.SetGameStatus(GameStatus.Game);

        _savePanel.SetActive(false);
    }

    private void CheckInputData()
    {
        var lvlData = new LvlData();

        if (_inputName.text.Length == 0)
        {
            FormationError(_inputName.gameObject);
            return;
        }
        else
        {
            lvlData.Name = _inputName.text;
        }

        if (_stepCount.text.Length == 0)
        {
            FormationError(_stepCount.gameObject);
            return;
        }
        else
        {
            lvlData.StepCount = int.Parse(_stepCount.text);
        }

        if (_scoreForStars.text.Length == 0)
        {
            FormationError(_scoreForStars.gameObject);
            return;
        }
        else
        {
            lvlData.ScoreForStars = int.Parse(_scoreForStars.text);
        }

        if (!_levelTargetsToggles.GetFirstActiveToggle())
        {
            FormationError(_levelTargetsToggles.gameObject);
            return;
        }
        else
        {
            switch (_levelTargetsToggles.GetFirstActiveToggle().name)
            {
                case "Points":
                    lvlData.LevelTarget = LevelTarget.Points;
                    if (_pointsTargetInputField.text.Length == 0)
                    {
                        FormationError(_pointsTargetInputField.gameObject);
                        return;
                    }

                    lvlData.PointsTarget = int.Parse(_pointsTargetInputField.text);
                    break;
                case "Tokens":
                    lvlData.LevelTarget = LevelTarget.Tokens;

                    if (!CheckTokensTargetData())
                    {
                        return;
                    }

                    var tokenTargets = new List<TokenTarget>();
                    
                    foreach (var constructItem in _tokensTargetInputScreen.ActiveObjects)
                    {
                        var tokenTarget = new TokenTarget();
                        
                        tokenTarget.Count = int.Parse(constructItem.TokenCount.text);
                        tokenTarget.Sprite = constructItem.SpriteToken;
                        tokenTarget.Type = constructItem.Type;
                        
                        tokenTargets.Add(tokenTarget);
                    }

                    lvlData.TokenTargets = tokenTargets;
                    break;
            }
        }

        Save(lvlData);
    }

    private bool CheckTokensTargetData()
    {
        foreach (var constructItem in _tokensTargetInputScreen.ActiveObjects)
        {
            if (constructItem.TokenCount.text.Length == 0)
            {
                FormationError(constructItem.TokenCount.gameObject);
                return false;
            }
        }

        return true;
    }

    private void FormationError(GameObject error)
    {
        _saveLvl.interactable = false;

        error.transform.DOShakePosition(0.5f, Vector3.right * 250).OnComplete(() => _saveLvl.interactable = true);
    }
    
    private void FormationError(string errorMessage)
    {
        _saveLvl.interactable = false;

        _errorScreen.SetActive(true);
        _errorScreen.transform.localScale = Vector3.zero;

        _errorTxt.text = errorMessage;

        _errorScreen.transform.DOScale(Vector3.one, 1f).OnComplete(() =>
        {
            _errorScreen.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                _saveLvl.interactable = true;
                _errorScreen.SetActive(false);
            });
        });
    }

    private void ShowTargetsSettings(bool b)
    {
        switch (_levelTargetsToggles.GetFirstActiveToggle().name)
        {
            case "Points":
                _tokensTargetInputScreen.gameObject.SetActive(false);
                _pointsTargetInputScreen.SetActive(true);
                break;
            case "Tokens":
                _pointsTargetInputScreen.SetActive(false);
                _tokensTargetInputScreen.gameObject.SetActive(true);
                break;
            default:
                _pointsTargetInputScreen.SetActive(false);
                _tokensTargetInputScreen.gameObject.SetActive(false);
                break;
        }
    }

    private void Save(LvlData lvlData)
    {
        var field = new List<CellData>();
        var spawnPoints = new List<Vector3>();

        foreach (var constructCell in _constructCells)
        {
            if (constructCell.Cell)
            {
                var cellData = new CellData();

                cellData.Position = constructCell.Cell.transform.position;

                if (constructCell.Cell.ActualToken != null)
                {
                    cellData.TokenType = constructCell.Cell.ActualToken.Type.ToString();
                    cellData.TokenHp = constructCell.Cell.ActualToken.Hp;
                }
                else
                {
                    cellData.TokenType = "null";
                }


                field.Add(cellData);
            }
            else if (constructCell.SpawnPoint)
            {
                var cellData = new CellData();

                cellData.Position = constructCell.SpawnPoint.transform.position;

                spawnPoints.Add(constructCell.SpawnPoint.transform.position);
            }
        }

        if (spawnPoints.Count == 0)
        {
            FormationError("Please setUp spawn points");
            
            return;
        }

        lvlData.Name = _inputName.text;
        lvlData.Field = field;
        lvlData.SpawnPoints = spawnPoints;

        _lvlsConstructData.LvlsData.Add(lvlData);
        
        Debug.Log("SAVE");
    }
}