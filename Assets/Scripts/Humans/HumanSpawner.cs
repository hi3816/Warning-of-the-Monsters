using System.Collections;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private Human _human;

    public void StartSpawningHumans(int waveIdx)
    {
        StartCoroutine(SpawnHumansCoroutine(waveIdx));
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        int spawnedHumans = 0;
        WaitForSeconds spawnDelay = new WaitForSeconds(2.0f);
        int countPerUnit = waveIdx * 2;
        while (spawnedHumans < countPerUnit)
        {
            SpawnHuman(waveIdx);
            spawnedHumans++;
            yield return spawnDelay;
        }
    }

    private void SpawnHuman(int waveIdx)
    {
        GameObject obj = PoolManager.Instance.SpawnFromPool("Human", transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.WaveIdx = waveIdx;
    }
}