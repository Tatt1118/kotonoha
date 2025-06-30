using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title : MonoBehaviour
{
    [SerializeField] private RectTransform triangle;

    void Start()
    {
        triangle.DOAnchorPosY(triangle.anchoredPosition.y + 10f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnMoveTile()
    {
        SceneManager.LoadScene("SampleScene");

    }

    public void TileScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
