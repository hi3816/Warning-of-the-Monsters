using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class summonerMonster : Monster //졸개들을 불러 인간을 막는 몬스터(=병영타워)
{
    private Transform[] summonPositions;
    private Dictionary<string, int> _minionToSummon;

    private void Start()
    {
        InitializeSummonableMinions();
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
        
        if (_targetHumanList.Count == 0)
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

            Monster_Data.Minion_Data minionData = MonsterDataManager.Instance.GetMinionData(minionTag);
            if (minionData != null)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector3 randomOffset = Random.insideUnitSphere * 2.5f;
                    randomOffset.y = 1f;
                    Vector3 spawnPosition = transform.position + randomOffset;
                    if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        GameObject minion = PoolManager.Instance.SpawnFromPool(minionTag, hit.position, Quaternion.identity);
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
            }
        }
    }
}