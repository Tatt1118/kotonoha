using UnityEngine;

public class PhoneInputState : IGameState
{
    private GameStateMachine _statemachine;
    private PhoneInputPresenter _presenter;

    public PhoneInputState(GameStateMachine machine, PhoneInputPresenter presenter)
    {
        _presenter = presenter;
        _statemachine = machine;
    }

    public void Enter()
    {
        _statemachine.UIManager.ShowDialogueUI();

        //stateを切り替えるイベント発火
        _presenter.OnInputNumber += input =>
        {
            _statemachine.StartStory(input);
        };
        _presenter.InitPhoneInput();
    }

    public void Update() { }

    public void Exit() { }
}


