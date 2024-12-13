using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class HumanController : MonoBehaviour
{
    private HumanSO _humanData;
    private int _id;
    private float _speed;
    private float _cooldown;
    private float _minFatigueInflicted;
    private float _maxFatigueInflicted;
    
    public Animator animator;
    public NavMeshAgent Agent { get; private set; }
    public Transform TargetMonster { get; private set; }

    public HumanStateMachine StateMachine;
    public WalkHumanState WalkHumanState { get; private set; }
    public RunHumanState RunHumanState { get; private set; }
    public BattleHumanState BattleHumanState { get; private set; }
    
    private float _lastAttackTime;

    private void Awake()
    {
        _humanData = HumanDataLoader.Instance.GetHumanByName(gameObject);
        _speed = _humanData.speed;
        _cooldown = _humanData.cooldown;
        _minFatigueInflicted = _humanData.minFatigueInflicted;
        _maxFatigueInflicted = _humanData.maxFatigueInflicted;
        
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogAssertion("Animator not found");
        Agent = GetComponent<NavMeshAgent>();
        if (Agent == null)
            Debug.LogError("NavMeshAgent not found");
        // Sprite가 화면상에 보이도록 조정
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        Agent.speed = _speed;
        
        StateMachine = new HumanStateMachine();
        WalkHumanState = new WalkHumanState(this, StateMachine);
        RunHumanState = new RunHumanState(this);
        BattleHumanState = new BattleHumanState(this, StateMachine);
    }

    private void OnEnable()
    {
        // 초기화 설정
        Agent.enabled = false;
        if (StageManager.Instance.SpawnPoint == null) return;
        transform.position = StageManager.Instance.SpawnPoint.position;   // 시작 위치 설정
        ClearTargetMonster();   // 타겟 몬스터 삭제
        Agent.enabled = true;
        Agent.ResetPath();  // 경로 초기화
        StateMachine.ChangeState(WalkHumanState);   // 걷는 상태로 전환
        // 애니메이션 초기화
        animator.SetBool("IsBattle", false);
        animator.SetBool("IsRun", false);
    }
    
    private void Update()
    {
        StateMachine.CurrentHumanState?.Update();   // 상태 머신에서 현재 상태를 계속 갱신
        if (TargetMonster != null)  // 타겟 몬스터가 있으면
        {
            AnimationDirectionChange(TargetMonster.position);   // 몬스터 방향으로 회전
        }
        else    // 타겟 몬스터가 없으면
        {
            AnimationDirectionChange(Agent.steeringTarget); // 다음 목적지 방향으로 회전
        }
    }

    // 목표 지점 기준으로 애니메이션의 방향 전환하는 메서드
    private void AnimationDirectionChange(Vector3 targetPosition)
    {
        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }
    
    // 쿨타임 계산하여 공격 가능한 상태인지 확인하는 메서드
    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _cooldown;
    }
    
    // 몬스터의 피로도를 증가시키는 메서드
    public void PerformAttack()
    {
        if (TargetMonster == null) return;  // 타겟 몬스터 없으면 바로 반환
        
        // 피로도 변동 수치 범위내의 랜덤 값을 몬스터에 적용
        float randValue = Random.Range(_minFatigueInflicted, _maxFatigueInflicted);
        Monster monster;
        if (TargetMonster.gameObject.TryGetComponent(out monster))
        {
            monster.IncreaseFatigue(randValue);
        }
        else
        {
            Debug.LogWarning("TargetMonster not found");
        }
        _lastAttackTime = Time.time;    // 마지막 공격 시각 갱신
    }
    
    public void SetTargetMonster(Transform monster)
    {
        TargetMonster = monster;
    }

    public void ClearTargetMonster()
    {
        TargetMonster = null;
    }
}