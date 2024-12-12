using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private Human _human;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(1.5f);  // 인간 스폰 되는 간격
    
    private List<WaveData> _waveData = new List<WaveData>();
    [SerializeField] private PoolManager.PoolConfig[] poolConfigs; // 인간 풀

    private void Awake()
    {
        PoolManager.Instance.AddPoolS(poolConfigs);
    }
    
    private void Start()
    {
        _waveData = WaveDataLoader.Instance.WaveDataDict[DataManager.Instance.selectedStageIdx];
    }
    
    public void StartSpawningHumans(int waveIdx)
    {
        StartCoroutine(SpawnHumansCoroutine(waveIdx));
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        if (waveIdx-- == StageManager.Instance.totalWave)
            HumanManager.Instance.isLastWave = true;
        for (int i = 0; i < _waveData[waveIdx].HumanID.Count; i++)
        {
            for (int j = 0; j < _waveData[waveIdx].Count[i]; j++)
            {
                SpawnHuman(waveIdx, _waveData[waveIdx].HumanID[i]);
                yield return _spawnDelay;
            }
        }
    }
    
    private void SpawnHuman(int waveIdx, int humanId)
    {
        if (HumanManager.Instance.isLastWave)
            HumanManager.Instance.CountPerWave++;
        string humanType = ((HumanType)humanId).ToString();
        GameObject obj = PoolManager.Instance.SpawnFromPool(humanType, transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.SpawnedWaveIdx = waveIdx;
    }
}