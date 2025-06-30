using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private RectTransform triangleRect;
    [SerializeField] private GameObject obj;
    [SerializeField] private TextMeshProUGUI textToMove;
    [SerializeField] private float moveDuration = 7f;
    [SerializeField] private float offscreenY = -617.6f;
    [SerializeField] private string titleSceneName = "TitleScene";

    private RectTransform rect;

    private void Start()
    {
        // 三角形の揺れアニメ開始
        if (triangleRect != null)
        {
            triangleRect.DOAnchorPosY(triangleRect.anchoredPosition.y + 10f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            Debug.LogWarning("triangleRect が未設定です");
        }

        // textToMoveのRectTransform取得
        if (textToMove != null)
        {
            rect = textToMove.rectTransform;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI が設定されていません");
        }
    }

    // ボタン押した時に呼ぶ
    public void OnButtonClicked()
    {
        if (rect == null)
        {
            Debug.LogError("textToMove の RectTransform がありません");
            return;
        }

        // objを非表示にする
        if (obj != null)
        {
            obj.SetActive(false);
        }

        // テキストをゆっくり下に移動開始
        rect.DOAnchorPosY(offscreenY, moveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 移動完了したらタイトルシーンへ遷移
                SceneManager.LoadScene(titleSceneName);
            });
    }

}
