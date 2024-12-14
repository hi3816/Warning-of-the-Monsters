using System.Collections.Generic;
using UnityEngine;

public class WaveData
{
    public int WaveIdx;         // 웨이브 인덱스
    public List<int> HumanID;   // 웨이브 당 등장 인간 종류 리스트
    public List<int> Count;     // 종류에 따라 등장하는 인원 수 리스트
}
public class WaveDataLoader : SingletonBase<WaveDataLoader>
{
    // 스테이지 별 웨이브 정보를 저장하는 딕셔너리
    // key: 스테이지 인덱스, value: 스테이지에 포함된 웨이브 정보(WaveData)
    public Dictionary<int, List<WaveData>> WaveDataDict = new Dictionary<int, List<WaveData>>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    // 특정 스테이지 웨이브 데이터 로드하고 딕셔너리에 추가
    public void SetWaveDataIdxStage(int stageIdx)
    {
        switch (stageIdx)
        {
            case 0:
                WaveDataDict.Add(0, SetWaveDataListFromStage(Wave_Data.Stage1.GetList()));
                break;
            case 1:
                WaveDataDict.Add(1, SetWaveDataListFromStage(Wave_Data.Stage2.GetList()));
                break;
            default:
                Debug.LogAssertion("Wrong stage index. WaveData unloaded.");
                break;
        }
    }
    
    // 특정 스테이지의 웨이브 데이터 객체 리스트를 일반적인 WaveData 리스트로 변환
    // (웨이브 리스트 데이터를 묶어 스테이지의 웨이브 리스트로 생성)
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
    
    // 스테이지별 웨이브 데이터 파싱하여 일반적인 WaveData 인스턴스를 생성
    // (스테이지 내의 개별 웨이브 데이터 생성, Json -> class로 만드는 실질적 파싱 부분을 포함)
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