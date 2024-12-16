public class RunHumanState : IHumanState
{
    private HumanController _human;
    public int AvoidPriority { get; set; }

    public RunHumanState(HumanController human)
    {
        _human = human;
        AvoidPriority = 50;
    }

    public void Enter()
    {
        _human.animator.speed *= 5;
        _human.Agent.SetDestination(StageManager.Instance.SpawnPoint.position);
        _human.Agent.speed *= 1.5f;
        _human.Agent.avoidancePriority = AvoidPriority;
    }

    public void Update()
    {
        // 도망 상태에서 스폰 지점에 도착하면
        if (_human.Agent.remainingDistance <= _human.Agent.stoppingDistance)
        {
            //_human.ReturnHumanToPool(0.1f); // 풀로 반환
        }
    }

    public void Exit()
    {
        _human.Agent.ResetPath();
    }
}