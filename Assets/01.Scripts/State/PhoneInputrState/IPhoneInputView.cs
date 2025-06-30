using UnityEngine;

public interface IPhoneInputView
{
    public void Show();
    public void Hide();
    public void ShowCharacterMemo(string num);
    public void CloseCharacterMemo();
}
