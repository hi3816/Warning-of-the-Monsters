using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanDataLoader : SingletonBase<HumanDataLoader>
{
    private HumanSO[] _humanSOs;

    protected override void Awake()
    {
        base.Awake();
        _humanSOs = SetHumanSOs();
    }
    
    private HumanSO[] SetHumanSOs()
    {
        List<Human_Data.HumanData> humanDataList = Human_Data.HumanData.GetList();

        HumanSO[] humanSOs = new HumanSO[humanDataList.Count];
        for (int i = 0; i < humanSOs.Length; i++)
        {
            humanSOs[i] = ScriptableObject.CreateInstance<HumanSO>();
            humanSOs[i].name = $"HumanSO{i + 1}";
            humanSOs[i].id = humanDataList[i].id;
            humanSOs[i].maxFear = humanDataList[i].maxFear;
            humanSOs[i].minFatigueInflicted = humanDataList[i].minFatigueInflicted;
            humanSOs[i].maxFatigueInflicted = humanDataList[i].maxFatigueInflicted;
            humanSOs[i].cooldown = humanDataList[i].cooldown;
            humanSOs[i].speed = humanDataList[i].speed;
            humanSOs[i].lifeInflicted = humanDataList[i].lifeInflicted;
            humanSOs[i].coin = humanDataList[i].coin;
        }

        return humanSOs;
    }

    private HumanSO GetHumanByIndex(int idx)
    {
        if (_humanSOs == null)
        {
            _humanSOs = SetHumanSOs();
        }
        return _humanSOs[idx];
    }

    public HumanSO GetHumanByName(GameObject go)
    {
        go.name = go.name.Replace("(Clone)", "");
        int id = (int)((HumanType)Enum.Parse(typeof(HumanType), go.name));
        HumanSO humanData = GetHumanByIndex(id);

        return humanData;
    }
}
