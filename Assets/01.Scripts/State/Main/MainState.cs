using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainState : IGameState
{
    private GameStateMachine _machine;
    private IntroManager _introManager;
    private int textNumber = 0;

    public MainState(GameStateMachine machine, IntroManager introManager)
    {
        this._machine = machine;
        _introManager = introManager;
    }

    public void Enter()
    {
        _machine.UIManager.ShowMainUI();
        _introManager.SetText(textNumber);
        _introManager.OnChangeState += HandleStartButton;
        _introManager.UpdateText();
    }

    public void Exit()
    {
        _introManager.OnChangeState -= HandleStartButton;

    }

    public void Update()
    {
        _introManager.UpdateText();
    }
    private void HandleStartButton()
    {
        _machine.StartStory("1-1"); //  ステート遷移！
    }
}
