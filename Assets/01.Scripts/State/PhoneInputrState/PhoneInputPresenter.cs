using UniRx;
using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class PhoneInputPresenter
{
    private PhoneInputView _view;
    private PhoneInputModel _model;
    private PhoneInputManager _inputManager;
    private CompositeDisposable _dispose;
    public Action<string> OnInputNumber;
    private bool _isInit = false;
    private HashSet<string> _completeStory = new HashSet<string>();
    private HashSet<string> _playStoryId = new HashSet<string>();

    /// <summary>
    /// コンストラクタ・初期設定 Presenterでモデル・UI・ステート諸々使用したい
    /// 外部から値を渡してもらう。
    /// </summary>
    public PhoneInputPresenter(
        PhoneInputView view,
        PhoneInputModel model,
        PhoneInputManager inputManager,
        CompositeDisposable disposables
    )
    {//すでnewしてインスタンス化されたものをメンバ変数に格納
        _view = view;
        _model = model;
        _inputManager = inputManager;
        _dispose = disposables;
    }

    public void InitPhoneInput()
    {
        if (_isInit) return;
        SubscribeInput();
        _isInit = true;
    }

    public void SubscribeInput()
    {
        _dispose.Clear();//購読済みなら解除して重複を阻止
        _inputManager.OnInput
            .Where(input => input.Length == 4)
            .Subscribe(async input =>
            {
                HandlePhoneInput(input);
                await UniTask.NextFrame();
                _inputManager.ClearInput();
            })
            .AddTo(_dispose);
    }

    /// <summary>
    /// modelの正誤判定処理とボタンの取得値を比較し、エラーが出たらViewに通知
    /// </summary>
    private void HandlePhoneInput(string input)
    {
        if (!_model.NumberJudge(input))
        {
            _view.Show(); // 無効な番号
            return;
        }

        string storyId = _model.GetStoryID(input);
        if (storyId == null) return;

        if (_playStoryId.Contains(storyId))
        {
            _view.Show(); // 既に入力済み
            return;
        }

        // 入力順チェック
        var storyOrder = _model.GetStoryOrderInCurrentPhase();
        int expectedIndex = _completeStory.Count(id => id.StartsWith($"{_model.GetCurrentPhase()}-"));

        if (expectedIndex >= storyOrder.Count || storyOrder[expectedIndex] != storyId)
        {
            _view.Show(); 
            return;
        }

        //正常に受け付けた場合の処理
        _playStoryId.Add(storyId);
        _completeStory.Add(storyId);

        if (storyId == "1-2")
        {
            _view.ShowCharacterMemo("1-2");
        }
        else if (storyId == "3-2")
        {
            _view.ShowCharacterMemo("3-2");
        }

        string prefix = $"{_model.GetCurrentPhase()}-";
        int storyCountInPhase = _model.GetStoryCountInCurrentPhase();
        if (_playStoryId.Count(id => id.StartsWith(prefix)) == storyCountInPhase)
        {
            _model.AdvancePhase(); // 全部再生したらフェーズ進行
        }

        OnInputNumber?.Invoke(storyId);
        _view.CloseCharacterMemo();
    }
}
