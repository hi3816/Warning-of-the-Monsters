using System.Collections.Generic;
using DataTable;
using UnityEngine;
using UnityEngine.AI;

public class HumanController : MonoBehaviour
{
    private float _speed;
    public float Cooldown { get; private set; }
    public float MinFatigueInflicted { get; private set; }
    public float MaxFatigueInflicted { get; private set; }
    public Animator animator;
    public NavMeshAgent Agent { get; private set; }
    public List<Transform> targetMonsterList = new List<Transform>();
    public Transform TargetMonster { get; private set; }
    public int SpawnedPointIdx { get; set; }
    public Transform humanEffect;
    public ParticleSystem attackParticle;
    public HumanStateMachine stateMachine;
    public WalkHumanState WalkHumanState { get; private set; }
    public RunHumanState RunHumanState { get; private set; }
    public BattleHumanState BattleHumanState { get; private set; }
    public bool isSurprising;
    
    private void Awake()
    {
        // 애니메이터 & NavMeshAgent 세팅
        animator = GetComponentInChildren<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        
        // Sprite가 화면상에 보이도록 조정
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        
        // 상태머신 세팅
        stateMachine = new HumanStateMachine();
        WalkHumanState = new WalkHumanState(this);
        RunHumanState = new RunHumanState(this);
        BattleHumanState = new BattleHumanState(this);
    }

    private void OnEnable()
    {
        // 초기화 설정
        Agent.enabled = false;
        if (StageManager.Instance.StartPointList == null) return;
        transform.position = StageManager.Instance.StartPointList[SpawnedPointIdx].position;   // 시작 위치 설정
        ClearTargetMonster();   // 타겟 몬스터 삭제
        targetMonsterList.Clear();
        Agent.enabled = true;
        Agent.ResetPath();  // 경로 초기화
        Agent.speed = _speed;   // 속도 초기화
        stateMachine.ChangeState(WalkHumanState);   // 걷는 상태로 전환
        // 애니메이션 초기화
        animator.SetBool("IsWalk", true);
        animator.SetBool("IsBattle", false);
        animator.speed = 1.0f;
        // 파티클 초기화
        humanEffect = HumanParticle.Instance.humanEffect;
        attackParticle = HumanParticle.Instance.attackParticle;
    }
    
    private void Update()
    {
        stateMachine.CurrentHumanState?.Update();   // 상태 머신에서 현재 상태를 계속 갱신
        
        if (TargetMonster != null)  // 타겟 몬스터가 있으면
        {
            AnimationDirectionChange(TargetMonster.position);   // 몬스터 방향으로 회전
        }
        else    // 타겟 몬스터가 없으면
        {
            AnimationDirectionChange(Agent.steeringTarget); // 다음 목적지 방향으로 회전
        }
    }
    
    public void SetHumanData(Human_Data humanData)
    {
        _speed = humanData.speed;
        Cooldown = humanData.cooldown;
        MinFatigueInflicted = humanData.minFatigueInflicted;
        MaxFatigueInflicted = humanData.maxFatigueInflicted;
    }

    // 목표 지점 기준으로 애니메이션의 방향 전환
    private void AnimationDirectionChange(Vector3 targetPosition)
    {
        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }
    
    // 가장 가까이에 있는 타겟 몬스터 설정
    public void SetTargetMonster()
    {
        if (targetMonsterList.Count == 0) return;
        // 타겟 몬스터 리스트 요소가 1개이면 해당 몬스터를 타겟 몬스터로 설정
        if (targetMonsterList.Count == 1)
        {
            TargetMonster = targetMonsterList[0];
            return;
        }

        // 타겟 몬스터 리스트 요소가 2개 이상이면 거리를 비교하여 가장 가까이 있는 것을 타겟 몬스터로 설정
        TargetMonster = targetMonsterList[1];   // 비교 대상 초기화
        float distance = (TargetMonster.position - transform.position).magnitude;   // 기존 타겟 몬스터와의 거리
        for (int i = targetMonsterList.Count - 2; i >= 0; i--)
        {
            if (targetMonsterList[i] == null) continue; // 중간에 몬스터 사라지는 경우 예외 처리
            float newDistance = (targetMonsterList[i].transform.position - transform.position).magnitude;
            if (newDistance < distance) // 기존 타겟 몬스터보다 거리가 가까우면
            {
                distance = newDistance; // 최단 거리 갱신
                TargetMonster = targetMonsterList[i];   // 타겟 몬스터 변경
            }  
        }
    }

    public void ClearTargetMonster()
    {
        TargetMonster = null;
    }
    
    // 타겟 몬스터가 있는 방향으로 파티클 실행
    public void PlayAttackParticle()
    {
        if (TargetMonster == null) return;  // 타겟 몬스터 없으면 리턴
        
        humanEffect.transform.position = transform.position;    // 파티클 시작 위치 설정
        
        // 현재 위치에서 타겟 몬스터로의 방향 구하고 정규화
        Vector3 directionToTarget = (TargetMonster.position - gameObject.transform.position).normalized;

        // 방향 벡터 -> 라디안 -> 각도
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        humanEffect.rotation = Quaternion.Euler(0, 0, angle);   // 파티클 부모 오브젝트를 계산된 각도로 회전
        
        attackParticle.Play();
    }
}