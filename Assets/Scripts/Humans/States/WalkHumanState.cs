public class WalkHumanState : IHumanState
{
    private HumanController _human;
    private HumanStateMachine _stateMachine;

    public WalkHumanState(HumanController human)
    {
        _human = human;
        _stateMachine = human.stateMachine;
    }

    public void Enter()
    {
        _human.animator.SetBool("IsBattle", false); // 애니메이션 Walk로 전환
        _human.Agent.SetDestination(StageManager.Instance.EndPoint.position);   // 거점으로 이동하게 설정
    }

    public void Update()
    {
        if (_human.TargetMonster != null)   // 타겟 몬스터가 있으면
        {
            _stateMachine.ChangeState(_human.BattleHumanState); // 전투 상태로 전환
        }
    }

    public void Exit()
    {
        _human.Agent.ResetPath();
    }
}