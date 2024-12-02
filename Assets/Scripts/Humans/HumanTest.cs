using UnityEngine;

public class HumanTest : MonoBehaviour
{
    private Human _human;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private Transform spawnPoint;
    
    private void Awake()
    {
        _human = Instantiate(humanPrefab, spawnPoint.position, Quaternion.identity); // 스폰 위치에 생성
    }

    private void Update()
    {
        // TestCode
        // 스페이스바 누르면 웨이브 시작
        if (Input.GetKeyDown("space"))
        {
            _human.IsWaveStarted = true;
        }
    }
}
