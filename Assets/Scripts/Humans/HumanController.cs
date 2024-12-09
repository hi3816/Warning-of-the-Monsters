using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HumanController : MonoBehaviour
{
    private HumanSO _humanData;
    private float _cooldown;
    private float _minFatigueInflicted;
    private float _maxFatigueInflicted;
    
    public NavMeshAgent Agent { get; private set; }
    public Transform SpawnPoint { get; private set; }
    public Transform EndPoint { get; private set; }
    public Transform TargetMonster { get; private set; }

    public HumanStateMachine StateMachine;
    public WalkHumanState WalkHumanState { get; private set; }
    public RunHumanState RunHumanState { get; private set; }
    public BattleHumanState BattleHumanState { get; private set; }
    
    private float _lastAttackTime;
    private bool _isReturning;  // 풀에 반환중인 상태인지 체크

    private void Awake()
    {
        _humanData = CustomUtil.ResourceLoad<HumanSO>("SO/Human/HumanSO_0");
        _cooldown = _humanData.cooldown;
        _minFatigueInflicted = _humanData.minFatigueInflicted;
        _maxFatigueInflicted = _humanData.maxFatigueInflicted;
        
        SpawnPoint = GameObject.FindGameObjectWithTag("HumanSpawnPoint").transform;
        if (SpawnPoint == null)
            Debug.LogAssertion("Spawnpoint not found");
        EndPoint = GameObject.FindGameObjectWithTag("DestinationPoint").transform;
        if (EndPoint == null)
            Debug.LogWarning("Endpoint not found");
        
        Agent = GetComponent<NavMeshAgent>();
        if (Agent == null)
            Debug.LogError("No NavMeshAgent found");
        // Sprite가 화면상에 보이도록 조정
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        
        StateMachine = new HumanStateMachine();
        WalkHumanState = new WalkHumanState(this, StateMachine);
        RunHumanState = new RunHumanState(this);
        BattleHumanState = new BattleHumanState(this, StateMachine);
    }

    private void OnEnable()
    {
        // 초기화 설정
        Agent.enabled = false;
        transform.position = SpawnPoint.position;   // 시작 위치 설정
        // Debug.Log($"HumanContorller:{transform.position}");
        ClearTargetMonster();   // 타겟 몬스터 삭제
        _isReturning = false;   // 반환하고 있지 않은 상태로 전환
        Agent.enabled = true;
        Agent.ResetPath();  // 경로 초기화
        StateMachine.ChangeState(WalkHumanState);   // 걷는 상태로 전환
    }

    private void Update()
    {
        StateMachine.CurrentHumanState?.Update();   // 상태 머신에서 현재 상태를 계속 갱신
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
        // TargetMonster.GetComponent<Monster>().IncreaseFatigue(randValue);
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
    
    // 지연시간 이후에 인간을 풀로 반환하는 메서드
    public void ReturnHumanToPool(float delay)
    {
        if (_isReturning) return;   // 풀로 반환하는 중이면 실행 x
        
        _isReturning = true;
        StartCoroutine(ReturnHumanProcess(delay));
    }
    
    // 지연시간 이후에 인간을 풀로 반환하는 코루틴
    private IEnumerator ReturnHumanProcess(float delay)
    {
        // Debug.Log("Returning human process");
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.ReturnToPool("Human", this.gameObject);
    }
}