using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class summonerMonster : Monster //졸개들을 불러 인간을 막는 몬스터(=병영타워)
{
    private Transform[] _summonPositions;
    private Dictionary<string, int> _minionToSummon;
    private CircleCollider2D _collider;

    private void Start()
    {
        InitializeSummonableMinions();
        _collider = GetComponent<CircleCollider2D>();
    }

    private void InitializeSummonableMinions()
    {
        _minionToSummon = new Dictionary<string, int>();

        if (data.poolTag == "Lich")
        {
            _minionToSummon.Add("Skeleton", 2);
            _minionToSummon.Add("Bat", 1);
        }

        //add other summoner monster later
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
            string minionTag = minionEntry.Key;
            int count = minionEntry.Value;

            DataTable.Monster_Data minionData = DataManager4.Instance.GetMinionData(minionTag);
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