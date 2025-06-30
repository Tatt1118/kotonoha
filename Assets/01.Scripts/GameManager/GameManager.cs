using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PhoneInputManager inputManager;
    [SerializeField] private PhoneInputView _phoneInputview;
    [SerializeField] private StoryView _storyView;
    [SerializeField] private EndView endView;
    [SerializeField] private IntroManager introManager;

    private GameStateMachine _statemachine;
    private PhoneInputPresenter _phoneInputpresenter;
    private PhoneInputModel _phoneInputmodel;
    private StoryPresenter _storyPresenter;
    private StoryModel _storyModel;

    void Start()
    {
        var disposables = new CompositeDisposable();
        _phoneInputmodel = new PhoneInputModel();
        _phoneInputpresenter = new PhoneInputPresenter(_phoneInputview, _phoneInputmodel, inputManager, disposables);
        _storyModel = new StoryModel();
        _storyPresenter = new StoryPresenter(_storyView, _storyModel);
        var stateFactory = new GameStateFactory();
        _statemachine = new GameStateMachine(stateFactory, uiManager);
        _statemachine.SetEndView(endView);
        stateFactory.SetPresenter(_phoneInputpresenter, _storyPresenter);
        _statemachine.StartMain(introManager);
    }

    void Update()
    {
        _statemachine?.Update();
    }
}
