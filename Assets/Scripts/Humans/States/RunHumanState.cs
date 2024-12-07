using UnityEngine;

public class RunHumanState : IHumanState
{
    private readonly HumanController _human;
    private readonly HumanStateMachine _stateMachine;

    public RunHumanState(HumanController human, HumanStateMachine stateMachine)
    {
        _human = human;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _human.agent.SetDestination(_human.SpawnPoint.position);
    }

    public void Update()
    {
        if (_human.agent.remainingDistance <= _human.agent.stoppingDistance)
        {
            _human.ReturnHumanToPool(0.1f);
        }
    }

    public void Exit()
    {
        _human.agent.ResetPath();
    }
}