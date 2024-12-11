using System.Collections;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private Human _human;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(1.5f);  // 인간 스폰 되는 간격
    private int _spawnPerWave = 3;  // 웨이브 당 인간 스폰 수 조정값
    
    //private Wave_Data.Stage1 _waveData;
    [SerializeField] private PoolManager.PoolConfig[] poolConfigs; // 인간 풀

    private void Awake()
    {
        PoolManager.Instance.AddPoolS(poolConfigs);
        Debug.Log(WaveDataLoader.Instance.WaveDataList);
    }

    public void StartSpawningHumans(int waveIdx)
    {
        StartCoroutine(SpawnHumansCoroutine(waveIdx));
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        int spawnedHumans = 0;
        int countPerUnit = waveIdx * _spawnPerWave;
         while (spawnedHumans < countPerUnit)
         {
             SpawnHuman(waveIdx);
             spawnedHumans++;
             yield return _spawnDelay;
         }
         
        // for (int i = 0; i < _waveData.count.Count; i++)
        // {
        //     for (int j = 0; j < _waveData.count[i]; j++)
        //     {
        //         SpawnHuman(waveIdx, j);
        //         yield return _spawnDelay;
        //     }
        // }
    }

    private void SpawnHuman(int waveIdx)
    {
        // Test
        GameObject obj = PoolManager.Instance.SpawnFromPool("NormalHuman", transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.SpawnedWaveIdx = waveIdx;
    }
    
    private void SpawnHuman(int waveIdx, int humanId)
    {
        // Test
        string humanType = ((HumanType)humanId).ToString();
        GameObject obj = PoolManager.Instance.SpawnFromPool(humanType, transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.SpawnedWaveIdx = waveIdx;
    }
}