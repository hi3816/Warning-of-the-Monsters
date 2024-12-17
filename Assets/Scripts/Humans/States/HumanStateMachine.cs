public interface IHumanState
{
    void Enter();
    void Update();
    void Exit();
}

public class HumanStateMachine
{
    private IHumanState _currentHumanState;
    public void ChangeState(IHumanState newHumanState)
    {
        _currentHumanState?.Exit();
        _currentHumanState = newHumanState;
        _currentHumanState?.Enter();
    }

    public IHumanState CurrentHumanState => _currentHumanState;
}