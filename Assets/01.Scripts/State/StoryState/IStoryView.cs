using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEditor;

public interface IStoryView
{
    public IObservable<Unit> OnNextClicked { get; }//ユーザのクリック通知
    public void ShowText(string text, bool isMain, string keyWord);
    public void ShowMeidoImage(bool enabled);
    public void ShowTeacherImage(bool enabled);
    public void ShowWatashiImage(bool enabled);
    public void AnimateKeyWord(bool isMain, string keyWord);
    public void UpdateKeyword(ReactiveCollection<string> keyWords);
    void SetKeywordClickHandler(Action<string> handler);
    void SetwaitKeyword(bool isWait);
    bool IsKeywordUIOpen { get; }
    void ShowOutlineForCharacter(string characterName, bool isVisible);
    public void DisableEffects();
    public void OnDestroyEffect();
}

