using System.Collections;
using DataTable;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class summonerMonster : Monster //졸개들을 불러 인간을 막는 몬스터(=병영타워)
{
    public List<Minion> MinionList { get; private set; } = new List<Minion>(); 
    private List<(int minionId, string minionTag, int count)> _minionToSummon = new List<(int, string, int)>();

    private void Start()
    {
        InitializeSummonableMinions();
    }

    public void InitializeSummonableMinions()
    {
        ClearMinion();
        _minionToSummon.Clear();
        Summon_Data summonData;
        if (data.currentLevel < data.maxLevel) summonData = DataManager.Instance.GetSummonData(data.id * 1000);
        else summonData = DataManager.Instance.GetSummonData(data.monsterId);
        _minionToSummon.Add((summonData.minionId[0], summonData.minionTag[0], summonData.count[0]));
        _minionToSummon.Add((summonData.minionId[1], summonData.minionTag[1], summonData.count[1]));
        SummonMinions();
    }

    protected override void Scaring()
    {
        if (_lastScareTime >= data.cooldown)
        {
            SummonMinions();
            _lastScareTime = 0f;
            SetState(MonsterState.Idle);
        }
    }
    
    private void SummonMinions()
    {
        int totalCount = 0;
        foreach (var item in _minionToSummon)
        {
            totalCount += item.count;
        }
        if (totalCount == MinionList.Count) return;
        
        Vector3[] spawnOffsets = new Vector3[]
        {
            new Vector3(0.5f, 0f, 0f), // 오른쪽
            new Vector3(-0.5f, 0f, 0f), // 왼쪽
            new Vector3(0f, 0.5f, 0f), // 위
            new Vector3(0f, -0.5f, 0f), // 아래
            new Vector3(0.5f, 0.5f, 0f), // 우측상단
            new Vector3(-0.5f, 0.5f, 0f), // 좌측상단
            new Vector3(0.5f, -0.5f, 0f), // 우측하단
            new Vector3(-0.5f, -0.5f, 0f) // 좌측하단
        };

        foreach (var minionEntry in _minionToSummon)
        {
            int minionId = minionEntry.minionId;
            string minionTag = minionEntry.minionTag;
            int count = minionEntry.count;
            if (count == CalcMinionListCount(minionId)) return;
            else count -= CalcMinionListCount(minionId);

            Monster_Data minionData = DataManager.Instance.GetBaseMonsterById(minionId);
            if (minionData != null)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector3 randomOffset = spawnOffsets[Random.Range(0, spawnOffsets.Length)];
                    Vector3 spawnPosition = transform.position + randomOffset;
                    MinionSetPosition(spawnPosition, minionTag, minionData);
                }
            }
        }
    }
    
    private void MinionSetPosition(Vector3 position, string minionTag, Monster_Data minionData)
    {
        Minion minion = PoolManager.Instance.SpawnFromPool<Minion>(minionTag, position, Quaternion.identity);
        if (minion != null)
        {
            minion.transform.position = position;
            minion.InitializeMinion(minionData, this);
            MinionList.Add(minion);
            GameManager.Instance.AddActiveList(minion);
        }
    }

    private int CalcMinionListCount(int minionId)
    {
        int minionCount = 0;
        foreach (var minionList in MinionList)
        {
            if (minionList.data.id == minionId)
            {
                minionCount++;
            }
        }
        return minionCount;
    }

    public void RemoveMinion(Minion minion)
    {
        MinionList.Remove(minion);
    }

    private void ClearMinion()
    {
        if (MinionList == null) return;

        foreach (Minion minion in MinionList)
        {
            GameManager.Instance.RemoveActiveList(minion);
            PoolManager.Instance.ReturnToPool(minion.data.poolTag, minion);
        }
        MinionList.Clear();
    }

    public override void ReturnToVillage()
    {
        ClearMinion();
        base.ReturnToVillage();
    }
}