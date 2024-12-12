using System.Collections.Generic;

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

        int waveCount = rawWaveData.Count;
        for (int i = 0; i < waveCount; i++)
        {
            WaveData data = new WaveData();
            data.WaveIdx = i;
            data.HumanID = new List<int>(rawWaveData[i].humanId);
            data.Count = new List<int>(rawWaveData[i].count);
            waveDataList.Add(data);
        }

        return waveDataList;
    }

}