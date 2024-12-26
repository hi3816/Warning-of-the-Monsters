using System.Collections.Generic;
using UnityEngine;

public class DataManager3 : SingletonBase<DataManager3>
{
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    public DataTable.Human_Data GetHumanByIndex(int idx)
    {
        if (DataTable.Human_Data.GetList() == null)
        {
            Debug.LogAssertion($"Human data not Loaded");
            return null;
        }
        return DataTable.Human_Data.Human_DataList[idx];
    }
    
    public DataTable.Wave_Data GetWaveByIndex(int waveIdx)
    {
        if (DataTable.Wave_Data.GetList() == null)
        {
            Debug.LogAssertion($"Wave data not Loaded");
            return null;
        }
        if (DataTable.Wave_Data.Wave_DataMap.TryGetValue(waveIdx, out var waveData))
        {
            return waveData;
        }
        Debug.LogAssertion($"Wave data not Found: {waveIdx}");
        return null;
    }
}
