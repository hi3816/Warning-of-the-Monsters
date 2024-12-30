using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class Minion : Monster //졸개
{
    private NavMeshAgent _navMeshAgent;
    private summonerMonster _summonerMonster;

    public void InitializeMinion(DataTable.Monster_Data minionData, summonerMonster summoner)
    {
        data.fatigue = minionData.fatigue;
        data.minFearInflicted = minionData.minFearInflicted;
        data.maxFearInflicted = minionData.maxFearInflicted;
        data.cooldown = minionData.cooldown;
        data.humanDetectRange = minionData.humanDetectRange;
        data.humanScaringRange = minionData.humanScaringRange;
        data.walkSpeed = minionData.walkSpeed;

        _summonerMonster = summoner;

        _navMeshAgent.speed = minionData.walkSpeed;
        Animator.speed = _navMeshAgent.speed;
        SetState(MonsterState.Idle);
    }
    
    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }

    protected override void Update()
    {
        base.Update();
        switch (MonsterState)
        {
         case MonsterState.Idle:
             HandleWalking();
             break;
         case MonsterState.Walking:
             break;
        }
    }
    
    protected override void SetState(MonsterState state)
    {
        base.SetState(state);
        switch (MonsterState)
        {
            case MonsterState.Walking:
                Animator.SetBool("Walking", true);
                break;
        }
    }
    
    private void HandleWalking()
    {
        Transform nearestHuman = GetNearestHuman();
        float distanceToSummoner = Vector3.Distance(transform.position, _summonerMonster.transform.position);
        if (nearestHuman != null)
        {
            float distanceToHuman = Vector3.Distance(transform.position, nearestHuman.position);
            if (distanceToHuman <= data.humanDetectRange)
            {
                _navMeshAgent.SetDestination(nearestHuman.position);
                SetState(MonsterState.Walking);
                if (distanceToHuman <= data.humanScaringRange)
                {
                    SetState(MonsterState.Scaring);
                }
                
                if (distanceToSummoner > 5f) // 만약 미니언이 소환사와 너무 멀어지면
                {
                    _navMeshAgent.SetDestination(_summonerMonster.transform.position);
                    SetState(MonsterState.Walking);
                }
            }
        }
        else
        {
            SetState(MonsterState.Idle);
        }
    }

    public override void ReturnToVillage()
    {
        base.ReturnToVillage();
        _summonerMonster.RemoveMinion(this);
    }
}
