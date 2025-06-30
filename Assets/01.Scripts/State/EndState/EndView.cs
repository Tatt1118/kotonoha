using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject KeywordBox;
    [SerializeField] private GameObject endUi;
    [SerializeField] private TextMeshProUGUI omoi;
    [SerializeField] private TextMeshProUGUI hyougen;
    [SerializeField] private TextMeshProUGUI buki;
    [SerializeField] private TextMeshProUGUI mahou;
    [SerializeField] private TextMeshProUGUI monogatari;
    [SerializeField] private TextMeshProUGUI kitai;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private TextMeshProUGUI resultText;
    private bool isClick;

    public void EndShowUI()
    {
        if (endUi == null) return;

        // UIを表示し、クリック状態をリセット
        isClick = true;
        startText.text = "";
        endText.text = "";
        endUi.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObj = eventData.pointerCurrentRaycast.gameObject;
        // TextMeshProUGUIがついているか確認
        var clickedText = clickedObj.GetComponent<TextMeshProUGUI>();
        if (clickedText == null) return;
        // 既にstartTextが空ならそこにセット
        if (string.IsNullOrEmpty(startText.text))
        {
            startText.text = clickedText.text;
        }
        // startが埋まっていて、endが空ならendにセット
        else if (string.IsNullOrEmpty(endText.text))
        {
            endText.text = clickedText.text;
        }
    }

    public void ResetText()
    {
        startText.text = "";
        endText.text = "";
    }

    public void OnDecideButtonClick()
    {
        if (string.IsNullOrEmpty(startText.text) || string.IsNullOrEmpty(endText.text)) return;
        KeywordBox.SetActive(false);
        resultText.text = $"{startText.text}は、まるで{endText.text}だ。";
    }
}
