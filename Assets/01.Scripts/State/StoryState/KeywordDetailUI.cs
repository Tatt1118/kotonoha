using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class KeywordDetailUI : MonoBehaviour, IPointerClickHandler
{
    private Action<string> _onClick;
    public string _keyword;

    public void Initialize(string keyword, Action<string> onClick)
    {
        _keyword = keyword;
        _onClick = onClick;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke(_keyword);
    }
}

