using DataTable;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class summonerMonster : Monster //졸개들을 불러 인간을 막는 몬스터(=병영타워)
{
    private Transform[] _summonPositions;
    private List<(int minionId, string minionTag, int count)> _minionToSummon;
    private CircleCollider2D _collider;

    private void Start()
    {
        InitializeSummonableMinions();
        _collider = GetComponent<CircleCollider2D>();
    }

    public void InitializeSummonableMinions()
    {
        _minionToSummon = new List<(int, string, int)>();
        Summon_Data summonData;

        if (data.currentLevel < data.maxLevel) summonData = DataManager.Instance.GetSummonData(data.id * 1000);
        else summonData = DataManager.Instance.GetSummonData(data.monsterId);
        _minionToSummon.Add((summonData.minionId[0], summonData.minionTag[0], summonData.count[0]));
        _minionToSummon.Add((summonData.minionId[1], summonData.minionTag[1], summonData.count[1]));
    }

    protected override void Scaring()
    {
        if (Time.time - LastScareTime >= data.cooldown)
        {
            SummonMinions();
            LastScareTime = Time.time;
        }

        if (TargetHumanList.Count == 0)
        {
            SetState(MonsterState.Idle);
        }
    }

    private void SummonMinions()
    {
        foreach (var minionEntry in _minionToSummon)
        {
            int minionId = minionEntry.minionId;
            string minionTag = minionEntry.minionTag;
            int count = minionEntry.count;

            DataTable.Monster_Data minionData = DataManager.Instance.GetBaseMonsterById(minionId);
            if (minionData != null)
            {
                for (int i = 0; i < count; i++)
                {
                    int roof = 0;
                    while (true)
                    {
                        Vector3 randomOffset = Random.insideUnitSphere * _collider.radius;
                        Vector3 spawnPosition = transform.position + randomOffset;
                        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                        {
                            MinionSetPosition(hit.position, minionTag, minionData);
                            break;
                        }
                        else roof++;

                        if (roof == 30)
                        {
                            MinionSetPosition(spawnPosition, minionTag, minionData);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MinionSetPosition(Vector3 position, string minionTag, DataTable.Monster_Data minionData)
    {
        Vector3 pos = position;
        pos.z = 0f;
        GameObject minion = PoolManager.Instance.SpawnFromPool(minionTag, pos, Quaternion.identity);
        if (minion != null)
        {
            minion.SetActive(true);
            Minion minionComponent = minion.GetComponent<Minion>();
            if (minionComponent != null)
            {
                minionComponent.InitializeMinion(minionData);
            }
        }
    }
}