using System.Collections;
using UnityEngine;

public class Minion : Monster //졸개
{
    private summonerMonster _summonerMonster;
    protected Vector3 _targetPosition;

    private Coroutine _minionMove;

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
        float distanceToTarget = Vector2.Distance(startPos, targetPos);
        float threshold = 0.01f; // 목표 위치와의 최소 거리

        while (distanceToTarget > threshold)
        {
            Vector2 direction = (targetPos - transform.position).normalized;
            UpdateAnimatorParameters(direction);
            Vector3 moveStep = direction * data.walkSpeed * Time.deltaTime;
            transform.position += moveStep;
            _targetPosition = transform.position; // 주위에 인간 없을 때를 대비
            distanceToTarget = Vector2.Distance(transform.position, targetPos);
            yield return null;
        }

        // 최종 위치
        transform.position = targetPos;
    }
}