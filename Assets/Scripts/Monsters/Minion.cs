using UnityEngine;

public class Minion : Monster //졸개
{
    private summonerMonster _summonerMonster;
    private Vector3 _targetPosition;
    private float _stoppingDistance = 0.5f;

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
        SetState(MonsterState.Idle);
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
             MoveTowardsTarget();
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
                _targetPosition = nearestHuman.position;
                SetState(MonsterState.Walking);
                if (distanceToHuman <= data.humanScaringRange)
                {
                    SetState(MonsterState.Scaring);
                    return;
                }
                
                if (distanceToSummoner > 5f) // 만약 미니언이 소환사와 너무 멀어지면
                {
                    Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized * 1f;
                    _targetPosition = _summonerMonster.transform.position + offset;
                    UpdateAnimatorParameters(_summonerMonster.transform.position);
                    SetState(MonsterState.Walking);
                }
            }
        }
        else
        {
            Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized * 1f;
            _targetPosition = _summonerMonster.transform.position + offset;
            UpdateAnimatorParameters(_summonerMonster.transform.position);
            SetState(MonsterState.Walking);
        }
    }
    
    private void MoveTowardsTarget()
    {
        if (_targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.walkSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, _targetPosition) <= _stoppingDistance)
        {
            SetState(MonsterState.Idle);
        }
    }

    public override void ReturnToVillage()
    {
        base.ReturnToVillage();
        _summonerMonster.RemoveMinion(this);
        PoolManager.Instance.ReturnToPool<Minion>(data.poolTag, this);
    }
}
