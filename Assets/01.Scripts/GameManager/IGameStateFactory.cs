public interface IGameStateFactory
{
    IGameState CreateMainState(GameStateMachine machine,IntroManager introManager);
    IGameState CreateDialogueState(GameStateMachine machine);
    IGameState CreateStoryState(GameStateMachine machine, string storyId);
    IGameState CreateEndingState(GameStateMachine machine,  EndView endView);
}
