using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class StoryPresenter
{
    private StoryModel _model;
    private IStoryView _view;
    private bool _isWaitingForKeyword = false;
    public Action OnStoryFinished;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public StoryPresenter(IStoryView view, StoryModel model)
    {
        _model = model;
        _view = view;
    }

    /// <summary>
    /// ストーリーの初期化処理
    /// ストーリー番号を指定して、ストーリーデータを読み込み、キーワードの初期化を行う。
    /// </summary>
    /// <param name="storyNum"></param>
    public void InitStory(string storyNum)
    {
        _disposables.Clear();
        _model.LoadStoryData(storyNum);
        _view.UpdateKeyword(_model.CollectedKeywords); // // キーワード初期表示

        _model.CollectedKeywords
            .ObserveCountChanged()//要素数に変化あったら発火
            .Subscribe(_ => _view.UpdateKeyword(_model.CollectedKeywords))
            .AddTo(_disposables);

        //modelの現在のストーリーが変わったら会話を表示
        _model.CurrentStoryIndex
            .Where(index => index < _model.StoryDataList.Count)
            .Subscribe(_ => ShowCurrentTalk())
            .AddTo(_disposables);

        _model.IsFinished
            .Where(finished => finished) // true のときだけ
            .Subscribe(_ => OnStoryFinished?.Invoke()) // ← Presenter外へ通知
            .AddTo(_disposables); //

        //クリック処理　HACK:Keywordクリックとテキストクリックが同時にならないようにしたが、結構ごり押し
        _view.OnNextClicked
            .Where(_ => !_isWaitingForKeyword)//テキスト中にキーワードが表示されていたら次いかない
            .Where(_ => !_view.IsKeywordUIOpen)//キーワードが表示されているUIがある場合も次いかない
            .Where(_ => _model.CurrentStoryIndex.Value < _model.StoryDataList.Count)//.Countは全体数
            .Subscribe(_ => _model.NextText())
            .AddTo(_disposables);
        // キーワードクリックしたよ購読・処理
        _view.SetKeywordClickHandler(OnKeywordClicked);
    }

    /// <summary>
    /// 現在のセリフを表示する
    /// </summary>
    /// <returns></returns>
    private void ShowCurrentTalk()
    {
        int currentStoryIndex = _model.CurrentStoryIndex.Value;
        if (currentStoryIndex >= _model.StoryDataList.Count)
        {
            //_view.ShowSubImage(false);
            _view.ShowText("", false, null);
            _view.ShowText("", true, null);
            return;
        }
        var data = _model.StoryDataList[currentStoryIndex];
        switch (data.charaNum)
        {
            case "1":
                _view.ShowOutlineForCharacter("watashi", false);
                _view.ShowOutlineForCharacter("meido", false);
                _view.ShowOutlineForCharacter("teacher", false);
                break;
            case "2":
                _view.ShowMeidoImage(true);
                _view.ShowWatashiImage(false);
                _view.ShowTeacherImage(false);
                _view.ShowOutlineForCharacter("meido", true);
                break;
            case "3":
                _view.ShowTeacherImage(true);
                _view.ShowMeidoImage(false);
                _view.ShowOutlineForCharacter("teacher", true);

                break;
            case "4":
                _view.ShowWatashiImage(true);
                _view.ShowMeidoImage(false);
                _view.ShowTeacherImage(false);
                _view.ShowOutlineForCharacter("watashi", true);
                break;

        }
        bool isMain = (data.charaNum == "1");
        _isWaitingForKeyword = !string.IsNullOrEmpty(data.KeyWord?.Trim());
        _view.ShowText(data.talks, isMain, data.KeyWord);
    }

    /// <summary>
    /// キーワードがクリックされたときの処理
    /// </summary>
    /// <param name="keyword"></param>
    private void OnKeywordClicked(string keyword)
    {
        _model.AddKeyword(keyword);
        _isWaitingForKeyword = false;
        _view.SetwaitKeyword(false);
    }

    public void EndStateEffect()
    {
        _view.OnDestroyEffect();
    }
}
