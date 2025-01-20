using System.Collections.Generic;
using System.Threading;
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
            // 다른 Human이 타겟으로 설정하지 않은 경우에만 설정
            if (!CheckOtherHumanTarget(targetMonsterList[0]))
            {
                TargetMonster = targetMonsterList[0];
            }
            return;
        }

        TargetMonster = null;
        float closestDistance = float.MaxValue; // 초기값을 최대 거리로 설정

        for (int i = targetMonsterList.Count - 1; i >= 0; i--)
        {
            Transform monster = targetMonsterList[i];
            // 리스트 중 Null 값이 있으면 건너뛰기
            if (monster == null) continue;
            // 다른 Human이 이미 타겟으로 설정했으면 건너뛰기
            if (CheckOtherHumanTarget(monster)) continue;

            // 거리 계산
            float distance = (monster.position - transform.position).magnitude;
            // 더 가까운 몬스터를 찾으면 갱신
            if (distance < closestDistance)
            {
                closestDistance = distance;
                TargetMonster = monster;
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

    private bool CheckOtherHumanTarget(Transform monster)
    {
        // 복사본을 사용해 안전하게 작업
        List<Human> humans = GameManager.Instance.GetActiveHumans();
        foreach (var human in humans)
        {
            if (human == null) continue; // Null 체크 추가
            Transform targetMonster = human.controller?.TargetMonster;
            if (targetMonster != null && targetMonster == monster)
            {
                return true;
            }
        }
        return false;
    }
}