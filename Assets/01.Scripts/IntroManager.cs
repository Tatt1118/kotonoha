using System;
using TMPro;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private TextData textData;
    [SerializeField] private TextMeshProUGUI introText;
    private int textIndex;
    public Action OnChangeState;

    void Start()
    {
        SetText(textIndex);
    }

    public void UpdateText()
    {
        if (Input.GetMouseButtonDown(0))
        {
            textIndex++;
            introText.text = "";
            SetText(textIndex);
            ProgressionStory();

        }
    }

    public void SetText(int _textIndex)
    {
        if (textData == null || textData.stories == null)
        {
            Debug.LogError("textDataが設定されていません");
            return;
        }

        if (_textIndex < 0 || _textIndex >= textData.stories.Count)
        {
            Debug.LogWarning($"Index {_textIndex} は範囲外です");
            return;
        }

        var setIntroText = textData.stories[_textIndex];
        introText.text = setIntroText.introTextData;
    }

    public void ProgressionStory()
    {
        if (textIndex >= textData.stories.Count)
        {
            OnChangeState?.Invoke();
        }

    }

}
