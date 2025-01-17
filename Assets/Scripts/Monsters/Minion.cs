using System.Collections;
using UnityEngine;

public class Minion : Monster //졸개
{
    private summonerMonster _summonerMonster;
    private Coroutine _minionMove;
    protected Vector3 _targetPosition;

    protected override void Update()
    {
        base.Update();
        switch (_monsterState)
        {
             case MonsterState.Idle:
                 UpdateAnimatorParameters(Vector2.zero);
                 if (_targetHumanList.Count > 0)
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
        if (_targetHumanList.Count == 0)
        {
            Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized;
            _targetPosition = _summonerMonster.transform.position + offset;
            SetState(MonsterState.Idle);
        }
        if (_targetHumanList.Count > 0)
        {
            Transform nearestHuman = _targetHumanList[0].transform;
            float distanceToSummoner = Vector3.Distance(transform.position, _summonerMonster.transform.position);
            float distanceToHuman = Vector3.Distance(transform.position, nearestHuman.position);
            if (distanceToHuman <= data.humanDetectRange)
            {
                _targetPosition = nearestHuman.position;
                if (distanceToHuman <= data.humanScaringRange)
                {
                    SetState(MonsterState.Scaring);
                }
            }
            else if (distanceToSummoner > data.humanDetectRange)
            {
                Vector3 offset = (transform.position - _summonerMonster.transform.position).normalized;
                _targetPosition = _summonerMonster.transform.position + offset;
            }
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
        float threshold = 0.1f; // 목표 위치와의 최소 거리

        foreach (Collider2D collider in _collider2Ds)
        {
            collider.enabled = false;
        }
        _targetHumanList.Clear();

        while (distanceToTarget >= threshold)
        {
            Vector2 direction = (targetPos - transform.position).normalized;
            UpdateAnimatorParameters(direction);
            Vector3 moveStep = direction * data.walkSpeed * Time.deltaTime;
            transform.position += moveStep;
            _targetPosition = transform.position; // 주위에 인간 없을 때를 대비
            distanceToTarget = Vector2.Distance(transform.position, targetPos);
            yield return null;
        }

        foreach (Collider2D collider in _collider2Ds)
        {
            collider.enabled = true;
        }

        // 최종 위치
        transform.position = targetPos;
    }
}