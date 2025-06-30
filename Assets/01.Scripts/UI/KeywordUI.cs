using System;
using UnityEngine;
using UnityEngine.UI;

public class KeywordUI : MonoBehaviour
{
    [SerializeField] private GameObject _keywordDetailPanel;
    [SerializeField] private Button button;
    internal string _keyword;

    public bool IsOpen => _keywordDetailPanel.activeSelf;//イベントの通知のためにプロパティを公開
    //Actionでイベント通知して

    public void OpenUI()
    {
        _keywordDetailPanel.SetActive(true);
    }

    public void CloseUI()
    {
        _keywordDetailPanel.SetActive(false);
    }
}
