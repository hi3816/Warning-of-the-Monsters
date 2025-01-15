using System.Collections;
using UnityEngine;

public class Minion : Monster //졸개
{
    private summonerMonster _summonerMonster;
    protected Vector3 _targetPosition;

    private float _minionMoveDuration = 2f;
    private Coroutine _minionMove;

    public void InitializeMinion(DataTable.Monster_Data minionData, summonerMonster summoner)
    {
        data.fatigue = minionData.fatigue[data.currentLevel];
        data.minFearInflicted = minionData.minFearInflicted[data.currentLevel];
        data.maxFearInflicted = minionData.maxFearInflicted[data.currentLevel];
        data.cooldown = minionData.cooldown[data.currentLevel];
        data.humanDetectRange = minionData.humanDetectRange[data.currentLevel];
        data.humanScaringRange = minionData.humanScaringRange[data.currentLevel];
        data.walkSpeed = minionData.walkSpeed[data.currentLevel];

        _summonerMonster = summoner;
        _targetPosition = transform.position;
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
        float distanceToSummoner = Vector3.Distance(transform.position, _summonerMonster.transform.position);
        if (TargetHumanList.Count > 0)
        {
            Transform nearestHuman = TargetHumanList[0].transform;
            float distanceToHuman = Vector3.Distance(transform.position, nearestHuman.position);
            if (distanceToSummoner < 1.5f && distanceToHuman <= data.humanDetectRange)
            {
                _targetPosition = nearestHuman.position;
                if (distanceToHuman <= data.humanScaringRange)
                {
                    SetState(MonsterState.Scaring);
                }
            }
            else if (distanceToSummoner >= 3.5f)
            {
                Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized;
                _targetPosition = _summonerMonster.transform.position + offset;
            }
        }
        if (TargetHumanList.Count == 0)
        {
            Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized;
            _targetPosition = _summonerMonster.transform.position + offset;
            SetState(MonsterState.Idle);
        }
    }
    
    public override void ReturnToVillage()
    {
        base.ReturnToVillage();
        _summonerMonster.RemoveMinion(this);
        GameManager.Instance.RemoveActiveList(this);
        PoolManager.Instance.ReturnToPool<Minion>(data.poolTag, this);
    }
    
    public void ReturnToVillage(bool remove)
    {
        base.ReturnToVillage();
        if (remove)
        {
            _summonerMonster.RemoveMinion(this);
        }
        GameManager.Instance.RemoveActiveList(this);
        PoolManager.Instance.ReturnToPool<Minion>(data.poolTag, this);
    }

    public void MoveToTarget(Vector2 targetPos)
    {
        if (_minionMove != null) StopCoroutine(_minionMove);
        _minionMove = StartCoroutine(CoMoveToTarget(transform.position, targetPos));
    }

    private IEnumerator CoMoveToTarget(Vector3 startPos, Vector3 targetPos)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < _minionMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            float time = elapsedTime / _minionMoveDuration;
            Vector2 currentPosition = Vector2.Lerp(startPos, targetPos, time);
            transform.position = new Vector2(currentPosition.x, currentPosition.y); // Z값 유지
            yield return null;
        }

        // 최종 위치 보정
        transform.position = new Vector2(targetPos.x, targetPos.y);
    }
}