using System;
using UniRx;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("各画面のUI")]
    public GameObject mainUI;
    public GameObject talkUI;
    public GameObject endUI;
    public GameObject dialogueUI;
    [SerializeField] private UnityEngine.UI.Button button;

    public void ShowMainUI()
    {
        SetActiveUI(mainUI);
    }

    public void ShowTalkUI()
    {
        SetActiveUI(talkUI);
    }

    public void ShowDialogueUI()
    {
        SetActiveUI(dialogueUI);
    }

    public void ShowEndUI()
    {
        SetActiveUI(endUI);
    }

    public void SetActiveUI(GameObject target)
    {
        mainUI.SetActive(false);
        talkUI.SetActive(false);
        dialogueUI.SetActive(false);
        endUI.SetActive(false);
        if (target != null) target.SetActive(true);
    }
}
