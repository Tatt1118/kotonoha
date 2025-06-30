using System;
using UnityEngine;

public class StoryState : IGameState
{
    private GameStateMachine _statemachine;
    private readonly string _storyId;
    private StoryPresenter _presenter;

    //コンストラクタ
    public StoryState(GameStateMachine machine, StoryPresenter presenter, string storyId)
    {
        _storyId = storyId;
        _statemachine = machine;
        _presenter = presenter;
    }

    public void Enter()
    {
        _statemachine.UIManager.ShowTalkUI();
        _presenter.InitStory(_storyId);
        _presenter.OnStoryFinished = () =>
        {
            if (_storyId == "3-3")
            {
                _presenter.EndStateEffect();
                _statemachine.StartEnding();
            }
            else
            {
                _statemachine.StartDialogue();
            }
        };
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}
