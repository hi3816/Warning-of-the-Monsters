using System.Collections.Generic;
using UnityEngine;

public class HumanDataLoader : SingletonBase<HumanDataLoader>
{
    private HumanSO[] _humanSOs;

    private void Awake()
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
    
    public HumanSO GetHumanByIndex(int idx)
    {
        if (_humanSOs == null)
        {
            _humanSOs = SetHumanSOs();
        }
        return _humanSOs[idx];
    }
}
