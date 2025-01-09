using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HumanSpawner : SingletonBase<HumanSpawner>
{
    private int _curStageIdx;
    private int _curWaveIdx;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(2.5f);  // 인간 스폰 되는 간격
    private List<Transform> _spawnPoints = new List<Transform>();
    
    [SerializeField] private PoolManager.PoolConfig[] _poolConfigs; // 인간 풀

    private Coroutine _spawnCoroutine;
    
    protected override void Awake()
    {
        base.Awake();
        PoolManager.Instance.AddPools<Human>(_poolConfigs);
        _curStageIdx = DataManager.Instance.selectedStageIdx;
    }

    private void Start()
    {
        _spawnPoints = StageManager.Instance.StartPointList;
    }
    
    public void StartSpawningHumans(int waveIdx)
    {
        _curWaveIdx = ConvertWaveIdx(_curStageIdx, waveIdx);
        StartCoroutine(SpawnHumansCoroutine(waveIdx));  // 현재 웨이브의 인간 스폰 코루틴 실행
    }

    public void StopSpawningHumans()
    {
        StopAllCoroutines();    // 모든 스폰 코루틴 중지
        Destroy(gameObject);
        Destroy(this);
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
            // Debug.LogError($"Wave data is missing: {_curWaveIdx}");
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

        
        int spawnPointCount = _spawnPoints.Count;
        for (int i = 0; i < spawnPointCount; i++)
        {
            Human human = PoolManager.Instance.SpawnFromPool<Human>(humanType, _spawnPoints[i].transform.position, quaternion.identity);
            human.controller.SpawnedPointIdx = i;    
            human.SpawnedWaveIdx = waveIdx;
            GameManager.Instance.AddActiveList(human);
        }
    }
    
    private int ConvertWaveIdx(int stageIdx, int waveIdx)
    {
        return (stageIdx + 1) * 1000 + waveIdx - 1; // 현재 스테이지와 웨이브인덱스로 json 데이터에서의 idx 반환
    }
    
    private int GetLastWaveIdx(int stageIdx)
    {
        int totalWaves = StageManager.Instance.TotalWave;   // 현재 스테이지의 총 웨이브 개수

        if (totalWaves <= 0)
        {
            // Debug.LogAssertion($"TotalWave is invalid: {totalWaves}");
            return -1;
        }

        return ConvertWaveIdx(stageIdx, totalWaves);
    }
}