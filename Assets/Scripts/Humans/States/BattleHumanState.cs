public class BattleHumanState : IHumanState
{
    private HumanController _human;
    private HumanStateMachine _stateMachine;

    public BattleHumanState(HumanController human, HumanStateMachine stateMachine)
    {
        _human = human;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _human.Agent.SetDestination(_human.TargetMonster.position);
    }

    public void Update()
    {
        if (_human.TargetMonster == null)   // 타겟 몬스터가 없으면
        {
            _stateMachine.ChangeState(_human.WalkHumanState);   // 걷기 상태로 전환
            return;
        }

        if (_human.CanAttack()) // 공격 가능한 상태면
        {
            _human.PerformAttack(); // 공격을 실행
        }
    }

    public void Exit()
    {
        _human.Agent.ResetPath();
    }
    
}