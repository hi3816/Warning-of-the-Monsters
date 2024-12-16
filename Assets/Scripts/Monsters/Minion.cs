using UnityEngine;
using UnityEngine.AI;

public class Minion : Monster //졸개
{
    private NavMeshAgent _navMeshAgent;
    private Monster_Data.Monster_Data _minionData;

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
             
             break;
         case MonsterState.Walking:
             
             break;
        }
    }
    
    public void InitializeMinion(Monster_Data.Monster_Data minionData)
    {
        _minionData = minionData;
        data.id = minionData.id;
        data.fatigue = minionData.fatigue;
        data.minFearInflicted = minionData.minFearInflicted;
        data.maxFearInflicted = minionData.maxFearInflicted;
        data.cooldown = minionData.cooldown;
        data.humanDetectRange = minionData.humanDetectRange;
        data.humanScaringRange = minionData.humanScaringRange;
        data.walkSpeed = minionData.walkspeed;
        data.maxLevel = minionData.maxLevel;
        _navMeshAgent.speed = minionData.walkspeed;
        SetState(MonsterState.Walking);
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
