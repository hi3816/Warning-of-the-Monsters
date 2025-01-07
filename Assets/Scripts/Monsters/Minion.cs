using UnityEngine;

public class Minion : Monster //졸개
{
    private summonerMonster _summonerMonster;
    private Vector3 _targetPosition;
    
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
             transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.walkSpeed * Time.deltaTime);
             Vector2 direction = (_targetPosition - transform.position).normalized;
             UpdateAnimatorParameters(direction);
             break;
        }
    }
    
    protected override void SetState(MonsterState state)
    {
        base.SetState(state);
        switch (MonsterState)
        {
            case MonsterState.Idle:
                Animator.SetBool("Walking", false);
                break;
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
            }
            
            if (distanceToSummoner > 5f && Vector3.Distance(_targetPosition, _summonerMonster.transform.position) > 1f)
            {
                Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized * 1f;
                _targetPosition = _summonerMonster.transform.position + offset;
            }
        }
        else
        {
            Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized * 1f;
            _targetPosition = _summonerMonster.transform.position + offset;
            SetState(MonsterState.Walking);
        }
    }

    public override void ReturnToVillage()
    {
        base.ReturnToVillage();
        _summonerMonster.RemoveMinion(this);
        PoolManager.Instance.ReturnToPool<Minion>(data.poolTag, this);
    }
}