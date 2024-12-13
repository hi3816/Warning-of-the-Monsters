using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private Human _human;
    private int _curWaveIdx;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(1.5f);  // 인간 스폰 되는 간격
    
    private List<WaveData> _waveData = new List<WaveData>();
    [SerializeField] private PoolManager.PoolConfig[] poolConfigs; // 인간 풀

    private void Awake()
    {
        PoolManager.Instance.AddPoolS(poolConfigs);
        _curWaveIdx = DataManager.Instance.selectedStageIdx;
    }
    
    private void Start()
    {
        // 현재 스테이지의 웨이브 데이터 로드
        if (!WaveDataLoader.Instance.WaveDataDict.ContainsKey(_curWaveIdx))
            WaveDataLoader.Instance.SetWaveDataIdxStage(_curWaveIdx);
        _waveData = WaveDataLoader.Instance.WaveDataDict[_curWaveIdx];
    }
    
    public void StartSpawningHumans(int waveIdx)
    {
        StartCoroutine(SpawnHumansCoroutine(waveIdx));  // 현재 웨이브의 인간 스폰 코루틴 실행
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        if (waveIdx-- == StageManager.Instance.totalWave)   // 현재 웨이브가 마지막 웨이브면
            HumanManager.Instance.isLastWave = true;
        for (int i = 0; i < _waveData[waveIdx].HumanID.Count; i++)  // 현재 웨이브의 인간 종류만큼 반복
        {
            for (int j = 0; j < _waveData[waveIdx].Count[i]; j++)   // 해당 종류 인원수 만큼 반복
            {
                SpawnHuman(waveIdx, _waveData[waveIdx].HumanID[i]);
                yield return _spawnDelay;
            }
        }
    }
    
    private void SpawnHuman(int waveIdx, int humanId)
    {
        // 웨이브별 인간 인원수 관리 딕셔너리에서 현재 웨이브 인덱스의 키가
        // 있으면 증가시키고 없으면 현재 웨이브 인덱스를 키로, 값을 1로 추가
        if (!HumanManager.Instance.CountPerWave.TryAdd(waveIdx, 1))
            HumanManager.Instance.CountPerWave[waveIdx]++;

        string humanType = ((HumanType)humanId).ToString(); // 스폰할 인간 종류를 태그 문자열로 변환
        GameObject obj = PoolManager.Instance.SpawnFromPool(humanType, transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.SpawnedWaveIdx = waveIdx;
    }
}