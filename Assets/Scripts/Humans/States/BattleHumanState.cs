using UnityEngine;

public class BattleHumanState : IHumanState
{
    private readonly HumanController _human;
    private readonly HumanStateMachine _stateMachine;
    private float _attackCooldown = 2f;

    public BattleHumanState(HumanController human, HumanStateMachine stateMachine)
    {
        _human = human;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _human.agent.SetDestination(_human.TargetMonster.position);
    }

    public void Update()
    {
        if (_human.TargetMonster == null)
        {
            _stateMachine.ChangeState(_human.WalkHumanState);
            return;
        }

        if (_human.CanAttack())
        {
            _human.PerformAttack();
        }
    }

    public void Exit()
    {
        _human.agent.ResetPath();
    }
    
}