using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using Utility;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VolFx;

public class StoryView : MonoBehaviour, IPointerClickHandler, IStoryView
{
    [SerializeField] private string storyNum;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _subText;
    [SerializeField] private GameObject _meidoImage;
    [SerializeField] private GameObject _teacherImage;
    [SerializeField] private GameObject _watashiImage;
    [SerializeField] private RectTransform _keywordUI;
    [SerializeField] private GameObject _keywordPrefab;
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private Button _resetButton;
    [SerializeField] private KeywordDetailUI keywordDetailUI;
    [SerializeField] private KeywordUI keywordUI;
    [SerializeField] private GameObject keywordEffectPrefab;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private Volume volume;

    private Dictionary<string, GameObject> _characterImage;
    private readonly Subject<Unit> _onNextClicked = new Subject<Unit>();
    public IObservable<Unit> OnNextClicked => _onNextClicked;
    public bool IsKeywordUIOpen => keywordUI.IsOpen;// キーワードUIが開いているかどうか
    private Action<string> onKeywordClicked;
    private bool _inputKeyword = false;
    private HashSet<string> clickedKeywords = new HashSet<string>();
    private int _clickCount = 0;
    private Camera uicamera;
    private List<GameObject> _keywordPool = new List<GameObject>();
    private VhsVol vhs;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private Bloom bloom;
    private string _currentKeyword;

    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uicamera = canvas.worldCamera;

        _characterImage = new Dictionary<string, GameObject>
        {
            { "meido", _meidoImage },
            { "teacher", _teacherImage },
            { "watashi", _watashiImage }
        };

        if (volume.profile.TryGet(out VhsVol vhsVol))
            vhs = vhsVol;

        if (volume.profile.TryGet(out ColorAdjustments ca))
            colorAdjustments = ca;

        if (volume.profile.TryGet(out ChromaticAberration ab))
            chromaticAberration = ab;

        if (volume.profile.TryGet(out Bloom bl))
            bloom = bl;
    }

    void Update()
    {
        if (_inputKeyword) return;
        if (Input.GetMouseButtonDown(0))
        {   // UIのボタンなどにクリックが乗っていたら無視する
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            _onNextClicked.OnNext(Unit.Default);
        }
    }

    public void ShowText(string text, bool isMain, string keyword)
    {
        var label = isMain ? _mainText : _subText;
        // 既存アニメーションを確実に停止
        DOTween.Kill(label);

        label.text = InsertLinkTag(text, keyword);
        label.ForceMeshUpdate();//更新
        if (!string.IsNullOrEmpty(keyword))
            AnimateKeyWord(isMain, keyword);
    }

    public void ShowMeidoImage(bool enabled) => _meidoImage.SetActive(enabled);
    public void ShowTeacherImage(bool enabled) => _teacherImage.SetActive(enabled);
    public void ShowWatashiImage(bool enabled) => _watashiImage.SetActive(enabled);

    /// <summary>
    ///テキスト中のキーワードにアニメーション
    /// </summary>
    /// <param name="isMain"></param>
    /// <param name="keyWord"></param>
    public void AnimateKeyWord(bool isMain, string keyWord)
    {
        if (string.IsNullOrEmpty(keyWord)) return;

        var textlabel = isMain ? _mainText : _subText;
        textlabel.ForceMeshUpdate();//textmeshproを何か操作するときに更新が必要
        var textInfo = textlabel.textInfo;

        var animator = new DOTweenTMPAnimator(textlabel);//テキストのDotweenのアニメーションを作るときに使用
        string tweenId = $"{textlabel.GetInstanceID()}_{keyWord}";
        DOTween.Kill(tweenId);

        foreach (var link in textInfo.linkInfo)
        {
            if (link.GetLinkID() != keyWord) continue;

            for (int j = 0; j < link.linkTextLength; j++)
            {
                int index = link.linkTextfirstCharacterIndex + j;

                DOTween.Sequence()
                    .SetId(tweenId)
                    .SetDelay(UnityEngine.Random.Range(0.3f, 0.8f))
                    .Append(animator.DOShakeCharOffset(index, 0.3f, 5f, 0))
                    .SetLoops(-1);

                animator.DOColorChar(index, Color.gray, 0.3f).SetId(tweenId);
                animator.DOScaleChar(index, 1.3f, 0.3f).SetId(tweenId);
            }
        }
    }

    /// <summary>
    ///キーワードにリンクタグを付ける
    /// </summary>
    /// <param name="text"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public string InsertLinkTag(string text, string keyword)
    {
        if (string.IsNullOrEmpty(keyword)) return text;
        if (clickedKeywords.Contains(keyword))
        {
            // 色タグをリンクタグの内側に入れる.置き換える
            return text.Replace(keyword, $"<link=\"{keyword}\"><color=#ffffff>{keyword}</color></link>");
        }
        else
        {
            if (text.Contains($"<link=\"{keyword}\">")) return text;
            return text.Replace(keyword, $"<link=\"{keyword}\">{keyword}</link>");
        }
    }

    public void SetKeywordClickHandler(Action<string> handler)
    {
        onKeywordClicked = handler;
    }

    public void SetwaitKeyword(bool isWait)
    {
        _inputKeyword = isWait;
    }

    /// <summary>
    ////キーワードがクリックされる処理・イベントハンドラのinterface
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 screenPos = Input.mousePosition;
        Canvas canvas = _subText.canvas;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        int index = TMP_TextUtilities.FindIntersectingLink(_subText, screenPos, cam);
        if (index == -1)
        {
            _onNextClicked.OnNext(Unit.Default);
            return;
        }

        TMP_LinkInfo linkInfo = _subText.textInfo.linkInfo[index];
        string linkID = linkInfo.GetLinkID();
        if (!clickedKeywords.Contains(linkID))
        {
            clickedKeywords.Add(linkID);
            onKeywordClicked?.Invoke(linkID);

            string tweenId = $"{_subText.GetInstanceID()}_{linkID}";
            DOTween.Kill(tweenId);

            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }

            // 変更：再描画ではなく色フェード
            FadeOutKeywordColor(linkInfo);
            int startIndex = linkInfo.linkTextfirstCharacterIndex;
            int length = linkInfo.linkTextLength;

            // キーワードクリックしたときにキーワードをUIへ移動する処理
            Vector3 avg = Vector3.zero;
            TMP_CharacterInfo[] charInfo = _subText.textInfo.characterInfo;
            //キーワード全体の中心位置の算出、エフェクトの位置
            //ここは、キーワードが移動するときのスタートする場所・位置を確立したい
            for (int i = startIndex; i < startIndex + length; i++)
            {
                Vector3 bottomLeft = charInfo[i].bottomLeft;
                Vector3 topRight = charInfo[i].topRight;
                Vector3 center = (bottomLeft + topRight) / 2f;
                avg += center;
            }
            avg /= length;//ローカル座標
            //ローカル座標からワールド座標へavgを変換
            Vector3 worldPos = _subText.transform.TransformPoint(avg);
            //ワールドからスクリーン座標へ
            Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
            // Canvas座標に変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenCenter, cam, out Vector2 localStart);

            // UIエフェクト生成 文字残像
            GameObject effect = Instantiate(keywordEffectPrefab, canvasRect);
            TextMeshProUGUI effectText = effect.GetComponent<TextMeshProUGUI>();
            if (effectText != null)
            {
                effectText.text = linkID;
            }
            RectTransform effectRect = effect.GetComponent<RectTransform>();
            effectRect.anchoredPosition = localStart;

            // ここで、ゴールの地点の座標・場所を確立する。それでクリック位置スタートからここのゴールへ移動のレールをひく。
            Vector2 screenTarget = RectTransformUtility.WorldToScreenPoint(cam, targetUI.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenTarget, cam, out Vector2 localEnd);
            //DOTween処理
            effectRect.DOAnchorPos(localEnd, 1f).SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(effect)); // 終了後削除
        }
    }

    /// <summary>
    ///キーワードを格納するUIを作成し表示と更新・オブジェクトプールで節約
    /// </summary>
    public void UpdateKeyword(ReactiveCollection<string> keywords)
    {
        Vector2 parentSize = _keywordUI.rect.size;

        int maxColumns = 3; // 列数
        int maxRows = 5;    // 行数
        float cellWidth = 220f;  // 1セルの横幅（適宜調整）
        float cellHeight = 100f;  // 1セルの高さ（適宜調整）
        Vector2 startPos = new Vector2(0, 0); // 左上を原点にするなら調整

        foreach (var word in keywords)
        {
            // すでに表示されているキーワードはスキップ
            if (_keywordPool.Any(x => x.activeSelf && x.GetComponent<KeywordDetailUI>()._keyword == word))
                continue;

            GameObject itemGO = _keywordPool.FirstOrDefault(x => !x.activeSelf);
            if (itemGO == null)
            {
                itemGO = Instantiate(_keywordPrefab, _keywordUI);
                _keywordPool.Add(itemGO);
            }

            itemGO.transform.SetParent(_keywordUI, false);
            itemGO.SetActive(true);

            var text = itemGO.GetComponentInChildren<TextMeshProUGUI>();
            var rectTransform = itemGO.GetComponent<RectTransform>();
            var keywordUI = itemGO.GetComponent<KeywordDetailUI>();

            text.text = word;

            keywordUI.Initialize(word, ShowKeyword);
        }

        // ここで位置調整（全表示中のキーワードに対して）
        var activeItems = _keywordPool.Where(x => x.activeSelf).ToList();

        // 位置調整ループの部分だけ抜粋
        for (int i = 0; i < activeItems.Count; i++)
        {
            var rectTransform = activeItems[i].GetComponent<RectTransform>();
            int col = i % maxColumns;
            int row = i / maxColumns;

            if (row >= maxRows)
            {
                activeItems[i].SetActive(false);
                continue;
            }

            // ピボット・アンカーを左上に固定
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);

            float posX = col * cellWidth;
            float posY = -row * cellHeight;
            rectTransform.anchoredPosition = new Vector2(posX, posY);
        }
    }

    /// <summary>
    /// キーワードをグレーにする処理
    /// テキスト中のキーワードをクリックしたらグレーにしたいから
    /// </summary>
    private void FadeOutKeywordColor(TMP_LinkInfo linkInfo, float duration = 0.3f)
    {
        _subText.ForceMeshUpdate();
        TMP_TextInfo textInfo = _subText.textInfo;

        int startIndex = linkInfo.linkTextfirstCharacterIndex;
        int length = linkInfo.linkTextLength;

        for (int i = startIndex; i < startIndex + length; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            var colors = textInfo.meshInfo[meshIndex].colors32;

            Color32 startColor = colors[vertexIndex];
            Color32 targetColor = new Color32(200, 200, 200, 180);

            float t = 0f;
            DOTween.To(() => t, x =>
            {
                t = x;
                Color32 lerped = Color32.Lerp(startColor, targetColor, t);
                for (int j = 0; j < 4; j++)
                {
                    colors[vertexIndex + j] = lerped;
                }

                _subText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }, 1f, duration);
        }
    }

    private void ShowKeyword(string showKeyword)
    {
        if (_clickCount == 0)
        {
            _targetText.text = showKeyword;
            _currentKeyword = showKeyword;
            _clickCount++;
        }
    }

    /// <summary>
    /// アウトラインのアセット・キャラクター画像のアウトラインのオンオフ
    /// </summary>
    /// <param name="isOutside"></param>
    public void ShowOutlineForCharacter(string characterImage, bool isVisible)
    {
        if (!_characterImage.ContainsKey(characterImage)) return;
        GameObject character = _characterImage[characterImage];
        if (character == null) return;
        Outline outline = character.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = isVisible;
        }
    }

    private void VolumeEffect(string keyword)
    {
        switch (keyword)
        {
            case "レトロ":
                if (colorAdjustments != null) colorAdjustments.active = true;
                break;

            case "めまい":
                if (chromaticAberration != null) chromaticAberration.active = true;
                break;

            case "ノイズ":
                if (vhs != null) vhs.active = true;
                vhs._weight.value = 0.136f;
                break;

            case "夢":
                if (bloom != null) bloom.active = true;
                bloom.intensity.Override(0.7f);
                break;
        }
    }

    /// <summary>
    /// 全てのエフェクトをオフに戻す
    /// </summary>
    public void DisableEffects()
    {
        if (vhs != null) vhs._weight.value = 0f;
        if (colorAdjustments != null) colorAdjustments.active = false;
        if (chromaticAberration != null) chromaticAberration.active = false;
        if (bloom != null) bloom.active = false;
        _targetText.text = "";
        _clickCount = 0;
    }

    public void OnEffect()
    {
        VolumeEffect(_currentKeyword);
    }

    /// <summary>
    ///シーンがTitleへ行くときに全消し用
    /// </summary>
    public void OnDestroyEffect()
    {
        if (volume == null || volume.gameObject == null) return;

        if (vhs != null)
        {
            vhs._weight.Override(0f);
            vhs.active = false;
        }

        if (bloom != null)
        {
            bloom.intensity.Override(0f);
            bloom.active = false;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.active = false;
        }

        if (chromaticAberration != null)
        {
            chromaticAberration.active = false;
            chromaticAberration.intensity.Override(0f);
        }

        _targetText.text = "";
        _clickCount = 0;

        Destroy(volume.gameObject);
    }
}

