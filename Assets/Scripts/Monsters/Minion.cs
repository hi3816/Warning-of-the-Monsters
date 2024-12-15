using UnityEngine;
using UnityEngine.AI;

public class Minion : Monster //졸개
{
    private NavMeshAgent _navMeshAgent;
    private Monster_Data.Minion_Data _minionData;
    
    public void InitializeMinion(Monster_Data.Minion_Data minionData)
    {
        _minionData = minionData;
        data.Fatigue = minionData.fatigue;
        data.FearInflicted = minionData.fearInflicted;
        data.Cooldown = minionData.cooldown;
        data.HumanScaringRange = minionData.humanScaringRange;
        data.Speed = minionData.speed;
        
        _navMeshAgent.speed = minionData.speed;
        SetState(MonsterState.Walking);
    }
    
    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    protected override void Update()
    {
        base.Update();
        if (MonsterState == MonsterState.Walking)
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
