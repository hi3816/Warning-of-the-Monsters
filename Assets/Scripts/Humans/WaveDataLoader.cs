using System.Collections.Generic;
using UnityEngine;

public class WaveData
{
    public int WaveIdx;
    public List<int> HumanID;
    public List<int> Count;
}
public class WaveDataLoader : SingletonBase<WaveDataLoader>
{
    public Dictionary<int, List<WaveData>> WaveDataDict = new Dictionary<int, List<WaveData>>();
    public List<WaveData> WaveDataList;

    protected override void Awake()
    {
        base.Awake();

        WaveDataDict.Add(0, SetWaveDataListFromStage(Wave_Data.Stage1.GetList()));
        WaveDataDict.Add(1, SetWaveDataListFromStage(Wave_Data.Stage2.GetList()));
    }
    
    private List<WaveData> SetWaveDataListFromStage<T>(List<T> waveDatas)
    {
        List<WaveData> waveDataList = new List<WaveData>();

        foreach (var waveData in waveDatas)
        {
            WaveData newData = CreateWaveData(waveData);
            if (newData != null)
            {
                waveDataList.Add(newData);
            }
        }

        return waveDataList;
    }
    private WaveData CreateWaveData<T>(T waveData)
    {
        if (waveData is Wave_Data.Stage1 stage1Data)
        {
            return new WaveData
            {
                WaveIdx = stage1Data.waveIdx,
                HumanID = new List<int>(stage1Data.humanId),
                Count = new List<int>(stage1Data.count)
            };
        }
        else if (waveData is Wave_Data.Stage2 stage2Data)
        {
            return new WaveData
            {
                WaveIdx = stage2Data.waveIdx,
                HumanID = new List<int>(stage2Data.humanId),
                Count = new List<int>(stage2Data.count)
            };
        }
        else
        {
            Debug.LogAssertion($"Unsupported type {typeof(T).Name}");
            return null;
        }
    }
}