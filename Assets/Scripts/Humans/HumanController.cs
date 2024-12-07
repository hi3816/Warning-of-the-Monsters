using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public interface IHumanState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}

public class HumanController : MonoBehaviour
{
    public Human human;
    private IHumanState _currentState;
    public NavMeshAgent agent;

    public Transform spawnPoint;
    public Transform endPoint;
    public Monster targetMonster;

    private bool isReturning;
    private float _lastAttackTime;
    private float maxFear;
    public bool IsBattle => targetMonster != null;
    
    public TextMeshProUGUI stateText;

    private void Awake()
    {
        human = GetComponent<Human>();
        // 카메라 상에 캐릭터가 보이도록 축과 회전값 조정
        agent = TryGetComponent<NavMeshAgent>(out agent) ? agent : gameObject.AddComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        maxFear = human.humanData.maxFear;
    }

    private void OnEnable()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("HumanSpawnPoint").transform;
        gameObject.transform.position = spawnPoint.position;
        if (spawnPoint == null)
        {
            Debug.LogWarning("Cannot find spawn point");
        }
        endPoint = GameObject.FindGameObjectWithTag("DestinationPoint").transform;
        if (endPoint == null)
        {
            Debug.LogWarning("Cannot find end point");
        }
        isReturning = false;
        TransitionToState(new WalkState(this));
    }

    private void Update()
    {
        _currentState?.UpdateState();
    }

    public void TransitionToState(IHumanState newState)
    {
        _currentState?.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + human.humanData.cooldown;
    }

    public void PerformAttack()
    {
        if (targetMonster == null) return;
        
        float randValue = Random.Range(human.humanData.minFatigueInflicted, human.humanData.maxFatigueInflicted);
        targetMonster.IncreaseFatigue(randValue);
        _lastAttackTime = Time.time;
    }

    public void SetTargetMonster(Monster newTarget)
    {
        targetMonster = newTarget;
    }

    public void ResetTarget()
    {
        targetMonster = null;
    }
    
    public void StopMoving()
    {
        if (agent != null)
            agent.ResetPath();
    }

    public void ReturnHumanToPool()
    {
        if (isReturning) return;
        
        isReturning = true;
        StartCoroutine(ReturnHumanProcess());
    }
    
    private IEnumerator ReturnHumanProcess()
    {
        Debug.Log("Returning human process");
        yield return new WaitForSeconds(1.0f);
        PoolManager.Instance.ReturnToPool("Human", this.gameObject);
    }
}