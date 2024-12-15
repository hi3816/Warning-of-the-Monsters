using System;
using UnityEngine;
using UnityEngine.AI;

public class Minion : Monster //졸개
{
    private NavMeshAgent _navMeshAgent;
    private Monster_Data.Minion_Data _minionData;
    
    public void InitializeMinion(Monster_Data.Minion_Data minionData)
    {
        _minionData = minionData;
        _minionData.fatigue = minionData.fatigue;
        _minionData.fearInflicted = minionData.fearInflicted;
        _minionData.cooldown = minionData.cooldown;
        _minionData.humanDetectRange = minionData.humanDetectRange;
        _minionData.humanScaringRange = minionData.humanScaringRange;
        _minionData.speed = minionData.speed;
        _navMeshAgent.speed = minionData.speed;
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
        if (MonsterState == MonsterState.Wondering)
        {
            SetState(MonsterState.Walking);
        }
        else if (MonsterState == MonsterState.Walking)
        {
            WalkTowardsNearestHuman();
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
    
    private void WalkTowardsNearestHuman()
    {
        Transform nearestHuman = GetNearestHuman();
        if (nearestHuman != null)
        {
            _navMeshAgent.SetDestination(nearestHuman.position);
        }
        else
        {
            SetState(MonsterState.Idle);
        }
    }
}
