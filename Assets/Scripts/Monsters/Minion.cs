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
             UpdateAnimatorParameters(Vector2.zero);
             if (TargetHumanList.Count > 0)
             {
                 HandleWalking();
             }
             break;
         case MonsterState.Walking:
             transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.walkSpeed * Time.deltaTime);
             Vector2 direction = (_targetPosition - transform.position).normalized;
             UpdateAnimatorParameters(direction);
             if (TargetHumanList.Count == 0)
             {
                 MonsterState = MonsterState.Idle;
             }
             break;
         case MonsterState.Scaring:
             transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.walkSpeed * Time.deltaTime);
             Vector2 faceToTarget = (_targetPosition - transform.position).normalized;
             UpdateAnimatorParameters(faceToTarget);
             Scaring();
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
                    _targetPosition = nearestHuman.position;
                    SetState(MonsterState.Scaring);
                }
                // else if (distanceToSummoner < 5f)
                // {
                //     Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized * 1f;
                //     _targetPosition = _summonerMonster.transform.position + offset;
                //     SetState(MonsterState.Walking);
                // }
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