using UnityEngine;
using UnityEngine.AI;

public class Minion : Monster //졸개
{
    private NavMeshAgent _navMeshAgent;
    private Monster_Data.Minion_Data _minionData;
    
    public void InitializeMinion(Monster_Data.Minion_Data minionData)
    {
        _minionData = minionData;
        data.fatigue = minionData.fatigue;
        data.fearInflicted = minionData.fearInflicted;
        data.cooldown = minionData.cooldown;
        data.humanScaringRange = minionData.humanScaringRange;
        data.speed = minionData.speed;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    protected override void Update()
    {
        base.Update();
        switch (MonsterState)
        {
            case MonsterState.Walking:
                WalkTowardsNearestHuman();
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
    
    private void WalkTowardsNearestHuman()
    {
        Transform nearestHuman = GetNearestHuman();
        if (nearestHuman != null)
        {
            _navMeshAgent.SetDestination(nearestHuman.position);
            transform.position += nearestHuman.position * (_minionData.speed * Time.deltaTime);
        }
    }
}
