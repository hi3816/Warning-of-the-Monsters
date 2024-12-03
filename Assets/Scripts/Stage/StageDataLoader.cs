using System;
using System.Collections.Generic;
using UnityEngine;

public class StageDataLoader : MonoBehaviour
{
    public StageSO[] SetStageSOs()
    {
        List<Stage_Data.Stage_Data> stageDataList = Stage_Data.Stage_Data.GetList();

        StageSO[] stageSOs = new StageSO[stageDataList.Count];
        for(int i = 0; i < stageSOs.Length; i++)
        {
            stageSOs[i] = ScriptableObject.CreateInstance<StageSO>();
            stageSOs[i].name = $"StageSO{i + 1}";
            stageSOs[i].wave = stageDataList[i].wave;
            stageSOs[i].health = stageDataList[i].health;
            stageSOs[i].gold = stageDataList[i].gold;
            stageSOs[i].interWaveDelay = stageDataList[i].interWaveDelay;
        }

        return stageSOs;
    }
}
