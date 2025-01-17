using UnityEngine;
using UnityEngine.AI;

public class BattleHumanState : IHumanState
{
    private HumanController _human;
    private HumanStateMachine _stateMachine;
    private Transform _oldTarget;
    private float _minFatigueInflicted;
    private float _maxFatigueInflicted;
    private float _lastAttackTime;
    
    public BattleHumanState(HumanController human)
    {
        _human = human;
        _stateMachine = human.stateMachine;
        _minFatigueInflicted = human.MinFatigueInflicted;
        _maxFatigueInflicted = human.MaxFatigueInflicted;
    }

    public void Enter()
    {
        _human.animator.SetBool("IsBattle", true);
        _oldTarget = _human.TargetMonster;
        _human.Agent.SetDestination(_oldTarget.position);
        FixPosition();
        _lastAttackTime = Time.time;    // 전투 시작 이후부터 쿨타임 계산
    }

    public void Update()
    {
        if (_human.TargetMonster == null)   // 타겟 몬스터가 없으면
        {
            _stateMachine.ChangeState(_human.WalkHumanState);   // 걷기 상태로 전환
            return;
        }

        if (!isFixed()) // 고정되어 있지 않으면
        {
            FixPosition();  // 위치 고정 상태로 만들기
        }

        if (_oldTarget != _human.TargetMonster) // 기존 타겟 몬스터가 변경되면
        {
            _oldTarget = _human.TargetMonster;
            _human.Agent.ResetPath();
            _human.Agent.SetDestination(_oldTarget.position);   // 새로운 타겟 몬스터 향하도록 설정
        }

        if (Time.time >= _lastAttackTime + _human.Cooldown && !_human.isSurprising) // 공격 가능한 상태면
        {
            PerformAttack(); // 공격을 실행
        }
    }

    public void Exit()
    {
        _human.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        _human.Agent.ResetPath();
    }
    
    // 몬스터의 피로도를 증가시키기
    public void PerformAttack()
    {
        if (_human.TargetMonster == null) return;  // 타겟 몬스터 없으면 바로 반환
        
        // 피로도 변동 수치 범위내의 랜덤 값을 몬스터에 적용
        float randValue = Random.Range(_minFatigueInflicted, _maxFatigueInflicted);
        Monster monster;
        if (_human.TargetMonster.gameObject.TryGetComponent(out monster))
        {
            _human.PlayAttackParticle();
            _human.animator.SetTrigger("Attack");
            monster.IncreaseFatigue(randValue);
        }

        _lastAttackTime = Time.time;    // 마지막 공격 시각 갱신
    }

    private bool isFixed()
    {
        return _human.Agent.obstacleAvoidanceType != ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    // 다른 상태의 Agent에게 밀리지 않도록 고정시키기
    private void FixPosition()
    {
        // 낮은 회피 품질로 설정 후 우선 순위 높임
        _human.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        _human.Agent.avoidancePriority = 0;
    }
}