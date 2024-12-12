using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class summonerMonster : Monster //졸개들을 불러 인간을 막는 몬스터(=병영타워)
{
    [Header("Summoning Settings")]
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
            _minionToSummon.Add("Skeleton", 3);
            _minionToSummon.Add("Bar", 2);
        }

        //add other summonermonster later
    }

    protected override void Update()
    {
        base.Update();
        switch (MonsterState)
        {
            case MonsterState.Summoning:
                if (IsHumanClose() && Time.time - LastScareTime >= data.cooldown)
                {
                    SummonMinions();
                }

                break;
        }
    }

    protected override void SetState(MonsterState state)
    {
        base.SetState(state);
        switch (MonsterState)
        {
            case MonsterState.Summoning:
                Animator.SetBool("Summon", true);
                break;
        }
    }

    private bool IsHumanClose()
    {
        Transform nearestHuman = GetNearestHuman();
        if (nearestHuman == null) return false;

        float distance = Vector3.Distance(transform.position, nearestHuman.position);
        return distance <= data.humanScaringRange;
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
                int summonedCount = 0;

                foreach (Transform summonPosition in summonPositions)
                {
                    if (summonedCount >= count) break;
                    Vector3 randomOffset = Random.insideUnitSphere * 2f;
                    randomOffset.y = 0;
                    Vector3 potentialSpawnPosition = summonPosition.position + randomOffset;

                    if (NavMesh.SamplePosition(potentialSpawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        GameObject minion =
                            PoolManager.Instance.SpawnFromPool(minionTag, hit.position, Quaternion.identity);

                        if (minion != null)
                        {
                            Minion minionComponent = minion.GetComponent<Minion>();
                            if (minionComponent != null)
                            {
                                minionComponent.InitializeMinion(minionData);
                                summonedCount++;
                            }
                        }
                    }
                }
            }

            LastScareTime = Time.time;
        }
    }
}