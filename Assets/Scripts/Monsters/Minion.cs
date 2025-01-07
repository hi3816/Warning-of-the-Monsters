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
                 SetState(MonsterState.Walking);
             }
             break;
         case MonsterState.Walking:
             HandleWalking();
             transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.walkSpeed * Time.deltaTime);
             Vector2 direction = (_targetPosition - transform.position).normalized;
             UpdateAnimatorParameters(direction);
             if (TargetHumanList.Count == 0)
             {
                 SetState(MonsterState.Idle);
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
        if (TargetHumanList != null)
        {
            float distanceToHuman = Vector3.Distance(transform.position, nearestHuman.position);
            if (distanceToHuman <= data.humanDetectRange)
            {
                _targetPosition = nearestHuman.position;
                if (distanceToSummoner > 2.5f)
                {
                    Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized * 1f;
                    _targetPosition = _summonerMonster.transform.position + offset;
                    SetState(MonsterState.Walking);
                }
                else if (distanceToHuman <= data.humanScaringRange)
                {
                    SetState(MonsterState.Scaring);
                }
            }
        }
    }
    
    private Transform GetNearestHuman()
    {
        Collider2D[] detectedHumans = Physics2D.OverlapCircleAll(transform.position, data.humanDetectRange, LayerMask.GetMask("Human"));
        if (detectedHumans.Length > 0)
        {
            Transform nearestHuman = detectedHumans[0].transform;
            float minDistance = Vector2.Distance(transform.position, nearestHuman.position);
            foreach (Collider2D human in detectedHumans)
            {
                float distance = Vector2.Distance(transform.position, human.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestHuman = human.transform;
                }
            }
            return nearestHuman;
        }
        return null;
    }
    
    public override void ReturnToVillage()
    {
        base.ReturnToVillage();
        _summonerMonster.RemoveMinion(this);
        PoolManager.Instance.ReturnToPool<Minion>(data.poolTag, this);
    }
}