using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ユーザボタン操作
/// </summary>
public class NumberButton : MonoBehaviour
{
    [SerializeField] private Text displayText; //入力された番号を表示する
    [SerializeField] private string buttonNum;
    public event Action<string> OnButtonNum;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            ButtonNum();
        });
    }

    public void ButtonNum()
    {
        OnButtonNum?.Invoke(buttonNum);
    }
}
