using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneInputManager : MonoBehaviour
{
    [SerializeField] private NumberButton[] numberButtons;//番号ボタン格納
    [SerializeField] private Text text;
    private ReactiveProperty<string> currentInput = new ReactiveProperty<string>("");
    public IObservable<string> OnInput => currentInput;

    /// <summary>
    /// 各ボタンの値取得と格納・UIの更新
    /// </summary>
    void Start()
    {
        foreach (var btn in numberButtons)
        {
            btn.OnButtonNum += AddNumber;
        }

        currentInput.Subscribe(value =>
        {
            text.text = value;
        }).AddTo(this);
    }

    public void AddNumber(string num)
    {
        if (currentInput.Value.Length >= 4) return;
        currentInput.Value += num;
    }

    public void ClearInput()
    {
        currentInput.Value = "";
    }

}

