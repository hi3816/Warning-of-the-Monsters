using UnityEngine;

public class BattleHumanState : IHumanState
{
    private HumanController _human;
    private HumanStateMachine _stateMachine;
    public int AvoidPriority { get; set; }

    private float _minFatigueInflicted;
    private float _maxFatigueInflicted;

    private float _lastAttackTime;
    
    public BattleHumanState(HumanController human, HumanStateMachine stateMachine)
    {
        _human = human;
        _stateMachine = stateMachine;
        _minFatigueInflicted = _human.MinFatigueInflicted;
        _maxFatigueInflicted = _human.MaxFatigueInflicted;
        AvoidPriority = 0;
    }

    public void Enter()
    {
        _human.animator.SetBool("IsBattle", true);
        _human.Agent.SetDestination(_human.TargetMonster.position);
        _human.Agent.avoidancePriority = AvoidPriority;
    }

    public void Update()
    {
        if (_human.TargetMonster == null)   // 타겟 몬스터가 없으면
        {
            _stateMachine.ChangeState(_human.WalkHumanState);   // 걷기 상태로 전환
            return;
        }

        if (Time.time >= _lastAttackTime + _human.Cooldown) // 공격 가능한 상태면
        {
            PerformAttack(); // 공격을 실행
        }
    }

    public void Exit()
    {
        _human.Agent.ResetPath();
    }
    
    // 몬스터의 피로도를 증가시키는 메서드
    public void PerformAttack()
    {
        if (_human.TargetMonster == null) return;  // 타겟 몬스터 없으면 바로 반환
        
        // 피로도 변동 수치 범위내의 랜덤 값을 몬스터에 적용
        float randValue = Random.Range(_minFatigueInflicted, _maxFatigueInflicted);
        Monster monster;
        if (_human.TargetMonster.gameObject.TryGetComponent(out monster))
        {
            monster.IncreaseFatigue(randValue);
        }
        else
        {
            Debug.LogWarning("TargetMonster not found");
        }
        _lastAttackTime = Time.time;    // 마지막 공격 시각 갱신
    }
}