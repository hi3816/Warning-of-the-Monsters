using UnityEngine;

public class WalkHumanState : IHumanState
{
    private readonly HumanController _human;
    private readonly HumanStateMachine _stateMachine;

    public WalkHumanState(HumanController human, HumanStateMachine stateMachine)
    {
        _human = human;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _human.agent.SetDestination(_human.EndPoint.position);
    }

    public void Update()
    {
        if (_human.TargetMonster != null)
        {
            _stateMachine.ChangeState(_human.BattleHumanState);
            return;
        }
    }

    public void Exit()
    {
        _human.agent.ResetPath();
    }
}