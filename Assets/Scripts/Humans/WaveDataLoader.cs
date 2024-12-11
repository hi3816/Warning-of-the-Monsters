using System.Collections.Generic;
using Human_Data;
using UnityEngine;

public class WaveData
{
    public int WaveIdx;
    public List<int> HumanID;
    public List<int> Count;
}
public class WaveDataLoader : SingletonBase<WaveDataLoader>
{
    public List<WaveData> WaveDataList;

    private void Awake()
    {
        base.Awake();
        WaveDataList = SetWaveData();
    }
    
    private List<WaveData> SetWaveData()
    {
        List<Wave_Data.Stage1> rawWaveData = Wave_Data.Stage1.GetList();
        List<WaveData> waveDataList = new List<WaveData>();

        return waveDataList;
    }

}