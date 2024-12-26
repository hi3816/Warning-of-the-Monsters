using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private int _curStageIdx;
    private int _curWaveIdx;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(2.0f);  // 인간 스폰 되는 간격
    
    [SerializeField] private PoolManager.PoolConfig[] _poolConfigs; // 인간 풀

    private void Awake()
    {
        PoolManager.Instance.AddPoolS(_poolConfigs);
        _curStageIdx = DataManager.Instance.selectedStageIdx;
    }
    
    public void StartSpawningHumans(int waveIdx)
    {
        _curWaveIdx = ConvertWaveIdx(_curStageIdx, waveIdx);
        StartCoroutine(SpawnHumansCoroutine(waveIdx));  // 현재 웨이브의 인간 스폰 코루틴 실행
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        if (_curWaveIdx == GetLastWaveIdx(_curStageIdx))    // 현재 웨이브가 마지막 웨이브면
        {
            HumanManager.Instance.isLastWave = true;
        }
        
        DataTable.Wave_Data waveData = DataManager.Instance.GetWaveByIndex(_curWaveIdx);   // 현재 웨이브 데이터 정보 가져오기

        if (waveData == null)
        {
            Debug.LogError($"Wave data is missing: {_curWaveIdx}");
            yield break;
        }

        for (int i = 0; i < waveData.humanId.Count; i++)    // 현재 웨이브의 인간 종류만큼 반복
        {
            for (int j = 0; j < waveData.count[i]; j++) // 해당 종류 인원수 만큼 반복
            {
                SpawnHuman(waveData.waveIdx, waveData.humanId[i]);
                yield return _spawnDelay;
            }
        }
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
    
    private int ConvertWaveIdx(int stageIdx, int waveIdx)
    {
        return (stageIdx + 1) * 1000 + waveIdx - 1; // 현재 스테이지와 웨이브인덱스로 json 데이터에서의 idx 반환
    }
    
    private int GetLastWaveIdx(int stageIdx)
    {
        int totalWaves = StageManager.Instance.TotalWave;

        if (totalWaves <= 0)
        {
            Debug.LogAssertion($"TotalWave is invalid: {totalWaves}");
            return -1;
        }

        return ConvertWaveIdx(stageIdx, totalWaves);
    }
}