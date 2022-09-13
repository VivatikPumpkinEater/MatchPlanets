using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokensTargetSettings : MonoBehaviour
{
    [SerializeField] private TargetConstructItem _targetsItemPrefab;

    [SerializeField] private Transform _content;

    [SerializeField] private Button _addedObject;
    [SerializeField] private Button _deleteObject;

    [SerializeField] private TargetTokenSelect _tokensSelect;

    public List<TargetConstructItem> ActiveObjects {get;} = new List<TargetConstructItem>();

    private float _step;
    private void Awake()
    {
        _addedObject.onClick.AddListener(AddedObject);
        _deleteObject.onClick.AddListener(DeleteObject);
        
        AddedObject();
    }

    private void AddedObject()
    {
        if(ActiveObjects.Count < 5)
        {
            var targetItem = Instantiate(_targetsItemPrefab, _content.transform);
            ActiveObjects.Add(targetItem);
            
            targetItem.IconButton.onClick.AddListener(() =>SelectedToken(targetItem));
        }
    }

    private void DeleteObject()
    {
        if (ActiveObjects.Count > 1)
        {
            Destroy(ActiveObjects[^1].gameObject);
            ActiveObjects.RemoveAt(ActiveObjects.Count - 1);
        }
    }

    private void SelectedToken(TargetConstructItem constructItem)
    {
        _tokensSelect.gameObject.SetActive(true);
        _tokensSelect.TargetConstructItem = constructItem;
    }
}