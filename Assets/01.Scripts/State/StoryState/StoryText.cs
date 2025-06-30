using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using Utility;
using System.Collections.Generic;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StoryText : MonoBehaviour, IPointerClickHandler
{
    [Header("Story Settings")]
    [SerializeField] private string storyNum;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI subText;
    //[SerializeField] private TextMeshProUGUI errorTextLabel;
    [SerializeField] private GameObject mainCharaImage;
    [SerializeField] private GameObject subCharaImage;

    [Header("KeyWord UI")]
    [SerializeField] private TextMeshProUGUI keyWordUI;

    private StoryDate[] storyDates;
    private int talkNum = 0;
    private bool canClick = true;
    private List<string> keyWords = new List<string>();
    private Camera uiCamera;

    void Awake()
    {
        if (keyWordUI == null) keyWordUI = GetComponent<TextMeshProUGUI>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;

        LoadStoryData();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            GetTalk().Forget();
        }
    }
    /// <summary>
    /// csvファイルを取り込む
    /// </summary>
    private void LoadStoryData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Story" + storyNum);
        if (textAsset == null) return;
        storyDates = CSVSerializer.Deserialize<StoryDate>(textAsset.text);
    }

    /// <summary>
    /// テキスト表示
    /// </summary>
    /// <param name="textLabel"></param>
    /// <param name="text"></param>
    /// <param name="keyWord"></param>
    /// <returns></returns>
    private async UniTask ShowText(TMP_Text textLabel, string text, string keyWord)
    {
        textLabel.text = "";
        canClick = false;

        if (!string.IsNullOrEmpty(keyWord))
        {
            text = InsertLink(text, keyWord);//<link>タグ文章の結果がくる
        }
        textLabel.DOText(text, 0.9f).SetEase(Ease.Linear);
        await UniTask.WaitUntil(() => textLabel.text == text);
        canClick = true;
        if (!string.IsNullOrEmpty(keyWord))
        {
            AnimateKeyWord(textLabel, keyWord);
        }
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    /// <summary>
    /// CSVの項目を取得して、項目順に実行
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid GetTalk()
    {
        while (talkNum < storyDates.Length)//CSVのストーリーの数
        {
            var data = storyDates[talkNum];
            Debug.Log(data);
            switch (data.charaNum)
            {
                case "1":
                    mainCharaImage.SetActive(true);
                    await ShowText(mainText, data.talks, data.KeyWord);
                    break;
                case "2":
                    subCharaImage.SetActive(true);
                    await ShowText(subText, data.talks, data.KeyWord);
                    break;
                default:
                    Debug.LogWarning("未知のcharaNum: " + data.charaNum);
                    break;
            }
            talkNum++;
        }
    }

    ///<summary>
    /// IPointerClickHandlerインターフェースよりオブジェクトのクリック関数
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 pos = Input.mousePosition;
        Canvas canvas = subText.canvas;
        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        int index = TMP_TextUtilities.FindIntersectingLink(subText, pos, camera);//文字の取得
        if (index == -1) return;
        TMP_LinkInfo linkInfo = subText.textInfo.linkInfo[index];
        string linkID = linkInfo.GetLinkID();
        if (!keyWords.Contains(linkID))
        {
            keyWords.Add(linkID);
            UpdateKeyWord();
        }
    }

    /// <summary>
    /// keywordのアニメーション処理
    /// </summary>
    public void AnimateKeyWord(TMP_Text textLabel, string keyWord)
    {
        textLabel.ForceMeshUpdate(); // テキスト情報を更新・文字列を操作する前にやる
        DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(textLabel);
        Sequence seq;

        var textInfo = textLabel.textInfo;

        for (int i = 0; i < textInfo.linkCount; i++)
        {
            var link = textInfo.linkInfo[i];
            if (link.GetLinkID() == keyWord)
            {
                int startIndex = link.linkTextfirstCharacterIndex;
                int length = link.linkTextLength;

                for (int j = 0; j < length; j++)
                {
                    int charIndex = startIndex + j;
                    if (charIndex >= tmproAnimator.textInfo.characterCount) continue;

                    seq = DOTween.Sequence();
                    seq.SetDelay(Random.Range(0.3f, 0.8f));
                    seq.Append(tmproAnimator.DOShakeCharOffset(charIndex, 0.3f, 5f, 0));
                    seq.SetLoops(-1);
                    tmproAnimator.DOColorChar(charIndex, Color.red, 0.3f);
                }
            }
        }
    }

    public void UpdateKeyWord()
    {
        // まずはメモ欄をクリア
        keyWordUI.text = "\n";
        // 登録されているキーワードを１行ずつ追加
        foreach (string word in keyWords)
        {
            keyWordUI.text += "・" + word + "\n";
        }
    }

    /// <summary>
    /// linkタグの処理
    /// </summary>
    private string InsertLink(string text, string keyWord)
    {
        if (string.IsNullOrEmpty(keyWord)) return text;
        if (text.Contains($"<link=\"{keyWord}\">")) return text;//重複阻止
        return text.Replace(keyWord, $"<link=\"{keyWord}\">{keyWord}</link>");
    }

    [System.Serializable]
    public class StoryDate
    {
        public string talkingChara;
        public string talks;
        public string charaNum;
        public string place;
        public string KeyWord;
    }
}
