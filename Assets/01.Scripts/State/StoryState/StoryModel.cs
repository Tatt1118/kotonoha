using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StoryModel
{
    private List<StoryData> _storyDataList = new List<StoryData>();
    private ReactiveProperty<bool> _isFinished = new ReactiveProperty<bool>(false);
    public IReadOnlyList<StoryData> StoryDataList => _storyDataList;
    public ReactiveCollection<string> CollectedKeywords { get; } = new ReactiveCollection<string>();
    public ReactiveProperty<int> CurrentStoryIndex { get; } = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<bool> IsFinished => _isFinished;

    /// <summary>
    /// ストーリーデータをCSVから読み込んでリストに変換する
    /// </summary>
    public void LoadStoryData(string storyNum)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Story" + storyNum);
        if (textAsset == null) return;

        _storyDataList = new List<StoryData>(CSVSerializer.Deserialize<StoryData>(textAsset.text));
        CurrentStoryIndex.Value = 0;
        _isFinished.Value = _storyDataList.Count == 0;
    }
    /// <summary>
    /// 次のセリフへ進める。最後まで行ったら終了フラグを立てる。
    /// １行終わったら次の1行進める処理
    /// </summary>
    public void NextText()
    {
        if (CurrentStoryIndex.Value < _storyDataList.Count - 1)
        {
            CurrentStoryIndex.Value++;
        }
        else
        {
            _isFinished.Value = true;
        }
    }

    /// <summary>
    /// 新しいキーワードならコレクションに追加
    /// </summary>
    public void AddKeyword(string keyword)
    {
        if (string.IsNullOrEmpty(keyword)) return;

        // 重複チェックしてから追加
        if (!CollectedKeywords.Contains(keyword))
        {
            CollectedKeywords.Add(keyword);
        }
    }

    [System.Serializable]
    public class StoryData
    {
        public string charaNum;
        public string talkingChara;
        public string talks;
        public string place;
        public string ImageDisp;
        public string KeyWord;
    }

}
