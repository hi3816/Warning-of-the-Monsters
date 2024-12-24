using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HumanSpawner : MonoBehaviour
{
    private Human _human;
    private int _curStageIdx;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(2.0f);  // 인간 스폰 되는 간격
    
    private List<WaveData> _waveData = new List<WaveData>();
    //private List<DataTable.Wave_Data> _waveData;
    [SerializeField] private PoolManager.PoolConfig[] _poolConfigs; // 인간 풀

    private void Awake()
    {
        PoolManager.Instance.AddPoolS(_poolConfigs);
        _curStageIdx = DataManager.Instance.selectedStageIdx;
    }
    
    private void Start()
    {
        // 현재 스테이지의 웨이브 데이터 로드
        if (!WaveDataLoader.Instance.waveDataDict.ContainsKey(_curStageIdx))
            WaveDataLoader.Instance.SetWaveDataIdxStage(_curStageIdx);
        _waveData = WaveDataLoader.Instance.waveDataDict[_curStageIdx];
        //DataManager3.Instance.GetWaveByIndex(_curStageIdx);
        //_waveData = DataManager3.Instance.GetAllWavesByStageIdx(_curStageIdx);
    }
    
    public void StartSpawningHumans(int waveIdx)
    {
        StartCoroutine(SpawnHumansCoroutine(waveIdx));  // 현재 웨이브의 인간 스폰 코루틴 실행
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        if (waveIdx-- == StageManager.Instance.TotalWave)   // 현재 웨이브가 마지막 웨이브면
            HumanManager.Instance.isLastWave = true;
        for (int i = 0; i < _waveData[waveIdx].humanID.Count; i++)  // 현재 웨이브의 인간 종류만큼 반복
        {
            for (int j = 0; j < _waveData[waveIdx].count[i]; j++)   // 해당 종류 인원수 만큼 반복
            {
                SpawnHuman(waveIdx, _waveData[waveIdx].humanID[i]);
                yield return _spawnDelay;
            }
        }
        
        // var currentWaveData = DataManager3.Instance.GetWaveByCompositeKey(_curStageIdx, waveIdx);
        //
        // if (currentWaveData != null)
        // {
        //     for (int i = 0; i < currentWaveData.humanId.Count; i++)  // 현재 웨이브의 인간 종류만큼 반복
        //     {
        //         for (int j = 0; j < currentWaveData.count[i]; j++)   // 해당 종류 인원수 만큼 반복
        //         {
        //             SpawnHuman(waveIdx, currentWaveData.humanId[i]);
        //             SpawnHuman(waveIdx, currentWaveData.humanId[i]);
        //             yield return _spawnDelay;
        //         }
        //     }
        // }
        // else
        // {
        //     Debug.LogAssertion($"Wave data not found for Stage: {_curStageIdx}, Wave: {waveIdx}");
        // }
    }
    
    private void SpawnHuman(int waveIdx, int humanId)
    {
        // 웨이브별 인간 인원수 관리 딕셔너리에서 현재 웨이브 인덱스의 키가
        // 있으면 증가시키고 없으면 현재 웨이브 인덱스를 키로, 값을 1로 추가
        if (!HumanManager.Instance.countPerWave.TryAdd(waveIdx, 1))
            HumanManager.Instance.countPerWave[waveIdx]++;

        string humanType = ((HumanType)humanId).ToString(); // 스폰할 인간 종류를 태그 문자열로 변환
        GameObject obj = PoolManager.Instance.SpawnFromPool(humanType, transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.SpawnedWaveIdx = waveIdx;
    }
}