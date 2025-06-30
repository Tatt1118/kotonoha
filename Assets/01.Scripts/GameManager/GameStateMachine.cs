public class GameStateMachine
{
    private IGameState currentState;
    public UIManager UIManager { get; private set; }
    private readonly IGameStateFactory _stateFactory;
    private EndView endView;

    // コンストラクタ：ステートの初期設定・初期化
    public GameStateMachine(IGameStateFactory stateFactory, UIManager uiManager)
    {
        _stateFactory = stateFactory;
        UIManager = uiManager;
    }

    /// <summary>
    /// 状態を切り替える
    /// </summary>
    public void ChangeState(IGameState newState)
    {
        currentState?.Exit();      // 現在の状態を終了
        currentState = newState;   // 新しい状態に切り替え
        currentState.Enter();      // 新しい状態を開始
    }

    /// <summary>
    /// 毎フレーム現在の状態を更新する
    /// </summary>
    public void Update()
    {
        currentState?.Update();
    }

    /// <summary>
    /// MainStateを開始する
    /// </summary>
    public void StartMain(IntroManager introManager)
    {
        ChangeState(_stateFactory.CreateMainState(this,introManager));
    }

    /// <summary>
    /// DialogueStateへ遷移する
    /// </summary>
    public void StartDialogue()
    {
        ChangeState(_stateFactory.CreateDialogueState(this));
    }

    /// <summary>
    /// StoryStateへ遷移する
    /// </summary>
    public void StartStory(string storyId)
    {
        ChangeState(_stateFactory.CreateStoryState(this, storyId));
    }

    /// <summary>
    /// EndStateへ遷移する
    /// </summary>
    public void StartEnding()
    {
        ChangeState(_stateFactory.CreateEndingState(this, endView));
    }

    public void SetEndView(EndView view)
    {
        endView = view;
    }
}
