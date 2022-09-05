using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class TokensTargetSettings : MonoBehaviour
{
    [SerializeField] private TargetConstructItem _targetsItemPrefab = null;

    [SerializeField] private Transform _content = null;

    [SerializeField] private Button _addedObject = null;
    [SerializeField] private Button _deleteObject = null;

    [SerializeField] private TargetTokenSelect _tokensSelect = null;

    public List<TargetConstructItem> ActiveObjects {get; private set; } = new List<TargetConstructItem>();

    private float _step = 0f;
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