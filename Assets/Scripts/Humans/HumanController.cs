using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class HumanController : MonoBehaviour
{
    private HumanSO _humanData;
    private int _id;
    private float _speed;
    public float Cooldown { get; private set; }
    public float MinFatigueInflicted { get; private set; }
    public float MaxFatigueInflicted { get; private set; }
    
    public Animator animator;
    public NavMeshAgent Agent { get; private set; }
    public Transform TargetMonster { get; private set; }

    public HumanStateMachine StateMachine;
    public WalkHumanState WalkHumanState { get; private set; }
    public RunHumanState RunHumanState { get; private set; }
    public BattleHumanState BattleHumanState { get; private set; }
    
    private void Awake()
    {
        // 데이터 세팅
        _humanData = HumanDataLoader.Instance.GetHumanByName(gameObject);
        _speed = _humanData.speed;
        Cooldown = _humanData.cooldown;
        MinFatigueInflicted = _humanData.minFatigueInflicted;
        MaxFatigueInflicted = _humanData.maxFatigueInflicted;
        
        // 애니메이터 & NavMeshAgent 세팅
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogAssertion("Animator not found");
        Agent = GetComponent<NavMeshAgent>();
        if (Agent == null)
            Debug.LogError("NavMeshAgent not found");
        // Sprite가 화면상에 보이도록 조정
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        
        // 상태머신 세팅
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
        Agent.speed = _speed;   // 속도 초기화
        StateMachine.ChangeState(WalkHumanState);   // 걷는 상태로 전환
        // 애니메이션 초기화
        animator.SetBool("IsBattle", false);
        animator.speed = 1.0f;
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
    
    public void SetTargetMonster(Transform monster)
    {
        TargetMonster = monster;
    }

    public void ClearTargetMonster()
    {
        TargetMonster = null;
    }
}