using UnityEngine;

public class EndState : IGameState
{
    private GameStateMachine machine;
    [SerializeField] EndView endView;

    public EndState(GameStateMachine machine, EndView endView)
    {
        this.machine = machine;
        this.endView = endView;
    }

    public void Enter()
    {
        if (machine == null) Debug.LogError("machine is null!");
        if (machine != null && machine.UIManager == null) Debug.LogError("machine.UIManager is null!");
        if (endView == null) Debug.LogError("endView is null!");
        machine.UIManager.ShowEndUI();
        endView.EndShowUI();
    }
    public void Update()
    {

    }
    public void Exit()
    {

    }
}

