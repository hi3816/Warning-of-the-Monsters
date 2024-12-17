public class RunHumanState : IHumanState
{
    private HumanController _human;

    public RunHumanState(HumanController human)
    {
        _human = human;
    }

    public void Enter()
    {
        _human.animator.speed *= 5; // 애니메이션 속도 증가
        _human.Agent.SetDestination(StageManager.Instance.SpawnPoint.position); // 시작 지점으로 되돌아가게 설정
        _human.Agent.speed *= 2.0f; // 이속 증가
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