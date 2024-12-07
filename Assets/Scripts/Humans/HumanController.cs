using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HumanController : MonoBehaviour
{
    public HumanSO humanData;
    public NavMeshAgent agent { get; private set; }
    public Transform SpawnPoint { get; private set; }
    public Transform EndPoint { get; private set; }
    public Transform TargetMonster { get; set; }

    public HumanStateMachine _stateMachine;
    
    public WalkHumanState WalkHumanState { get; private set; }
    public RunHumanState RunHumanState { get; private set; }
    public BattleHumanState BattleHumanState { get; private set; }
    
    private float _lastAttackTime;
    private bool isReturning;

    private void Awake()
    {
        humanData = CustomUtil.ResourceLoad<HumanSO>("SO/Human/HumanSO_0");
        
        SpawnPoint = GameObject.FindGameObjectWithTag("HumanSpawnPoint").transform;
        EndPoint = GameObject.FindGameObjectWithTag("DestinationPoint").transform;
        if (EndPoint == null)
            Debug.LogWarning("Endpoint not found");
        
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("No NavMeshAgent found");
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        _stateMachine = new HumanStateMachine();
        WalkHumanState = new WalkHumanState(this, _stateMachine);
        RunHumanState = new RunHumanState(this, _stateMachine);
        BattleHumanState = new BattleHumanState(this, _stateMachine);
    }

    private void OnEnable()
    {
        agent.enabled = false;
        transform.position = SpawnPoint.position;
        ClearTargetMonster();
        isReturning = false;
        Debug.LogWarning($"HumanContorller:{transform.position}");
        agent.enabled = true;
        agent.ResetPath();
        _stateMachine.ChangeState(WalkHumanState);
    }

    private void Update()
    {
        _stateMachine.CurrentHumanState?.Update();
    }
    
    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + humanData.cooldown;
    }
    
    public void PerformAttack()
    {
        if (TargetMonster == null) return;
        
        float randValue = Random.Range(humanData.minFatigueInflicted, humanData.maxFatigueInflicted);
        TargetMonster.GetComponent<Monster>().IncreaseFatigue(randValue);
        _lastAttackTime = Time.time;
    }
    
    public void SetTargetMonster(Transform monster)
    {
        TargetMonster = monster;
    }

    public void ClearTargetMonster()
    {
        TargetMonster = null;
    }
    
    public void ReturnHumanToPool(float delay)
    {
        if (isReturning) return;
        
        isReturning = true;
        StartCoroutine(ReturnHumanProcess(delay));
    }
    
    private IEnumerator ReturnHumanProcess(float delay)
    {
        Debug.Log("Returning human process");
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.ReturnToPool("Human", this.gameObject);
    }
}