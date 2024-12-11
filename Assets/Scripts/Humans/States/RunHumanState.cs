public class RunHumanState : IHumanState
{
    private HumanController _human;

    public RunHumanState(HumanController human)
    {
        _human = human;
    }

    public void Enter()
    {
        _human.Agent.SetDestination(_human.SpawnPoint.position);
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