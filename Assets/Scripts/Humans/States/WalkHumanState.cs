public class WalkHumanState : IHumanState
{
    private HumanController _human;
    private HumanStateMachine _stateMachine;
    public int AvoidPriority { get; set; }

    public WalkHumanState(HumanController human, HumanStateMachine stateMachine)
    {
        _human = human;
        _stateMachine = stateMachine;
        AvoidPriority = 51;
    }

    public void Enter()
    {
        _human.animator.SetBool("IsBattle", false);
        _human.Agent.SetDestination(StageManager.Instance.EndPoint.position);
        _human.Agent.avoidancePriority = AvoidPriority;
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