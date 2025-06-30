/// <summary>
/// Factoryパターン
/// 各ステートのインスタンス生成を一括管理
/// ほかの場所でステートインスタンス生成をバラバラにしていたことと生成ロジックをシンプルにしたかった
/// </summary>
public class GameStateFactory : IGameStateFactory
{
    private PhoneInputPresenter _phoneInputpresenter;
    private StoryPresenter _storyPresenter;

    public GameStateFactory() { }

    public void SetPresenter(PhoneInputPresenter phoneInputpresenter, StoryPresenter storyPresenter)
    {
        _phoneInputpresenter = phoneInputpresenter;
        _storyPresenter = storyPresenter;
    }

    public IGameState CreateMainState(GameStateMachine machine,IntroManager introManager)
    {
        return new MainState(machine,introManager);
    }

    public IGameState CreateDialogueState(GameStateMachine machine)
    {
        return new PhoneInputState(machine, _phoneInputpresenter);
    }

    public IGameState CreateStoryState(GameStateMachine machine, string storyId)
    {
        return new StoryState(machine, _storyPresenter, storyId);
    }

    public IGameState CreateEndingState(GameStateMachine machine, EndView endView)
    {
        return new EndState(machine,endView);
    }
}
