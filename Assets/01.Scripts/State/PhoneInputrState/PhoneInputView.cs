using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PhoneInputView : MonoBehaviour, IPhoneInputView
{
    [SerializeField] private TextData errorMessage;
    [SerializeField] private Text text;
    [SerializeField] private GameObject errorObj;
    [SerializeField] private Transform errorTransform;
    [SerializeField] private GameObject characterMemo;
    [SerializeField] private GameObject meidoKeywords;
    [SerializeField] private GameObject teacherKeywords;
    [SerializeField] private GameObject watashiKeywords;

    private const int displayMilliseconds = 1000;
    private bool isShowing = false;

    public void Show()
    {
        if (isShowing) return;
        isShowing = true;
        ShowErrorAsync().Forget();
    }

    public void Hide()
    {
        isShowing = false;
        errorObj.SetActive(false);
    }

    public async UniTask ShowErrorAsync()
    {
        isShowing = true;

        if (text == null || errorMessage == null || errorObj == null || errorTransform == null)
        {
            return;
        }

        text.text = errorMessage.Message;
        errorObj.SetActive(true);
        await UniTask.Delay(displayMilliseconds, DelayType.UnscaledDeltaTime);
        Hide();
    }


    public void ShowCharacterMemo(string num)
    {
        if (num == "1-2")
        {
            teacherKeywords.SetActive(true);
        }

        else if (num == "3-2")
        {
            watashiKeywords.SetActive(true);
        }
    }

    public void CloseCharacterMemo()
    {
        characterMemo.SetActive(false);
    }
}
