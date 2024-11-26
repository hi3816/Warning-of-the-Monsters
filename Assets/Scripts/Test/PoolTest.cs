using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PoolTest : MonoBehaviour
{
    public Transform triangleSpawnPoint;    // 삼각형 풀 생성 지점
    public Transform squareSpawnPoint;      // 사각형 풀 생성 지점
    public float activeTime;                // 활성화되는 시간

    private void Update()
    {
        //  SpaceBar 입력 시 삼각형과 사각형 오브젝트 풀에서 가져옴
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject triangle = PoolManager.Instance.SpawnFromPool("Triangle", triangleSpawnPoint.position, quaternion.identity);
            GameObject square = PoolManager.Instance.SpawnFromPool("Square", squareSpawnPoint.position, quaternion.identity);
            
            // 활성 시간 이후에 자동으로 풀에 반환하는 코루틴 시작
            StartCoroutine(ReleaseAfterTime("Triangle", triangle,activeTime));
            StartCoroutine(ReleaseAfterTime("Square", square, activeTime));
        }

        // Tab 입력 시 모든 오브젝트 풀 삭제
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PoolManager.Instance.DeleteAllPools();
        }
        
        // T Key 입력 시 삼각형 오브젝트 풀 삭제
        if (Input.GetKeyDown(KeyCode.T))
        {
            PoolManager.Instance.DeletePool("Triangle");
        }
        
        // S Key 입력 시 사각형 오브젝트 풀 삭제
        if (Input.GetKeyDown(KeyCode.S))
        {
            PoolManager.Instance.DeletePool("Square");
        }
    }

    private IEnumerator ReleaseAfterTime(string tag, GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        PoolManager.Instance.ReturnToPool(tag, go); // 풀에 오브젝트 반환
    }
}