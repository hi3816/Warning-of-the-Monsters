using UnityEngine;

public class HumanTest : MonoBehaviour
{
    private Human _human;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private Transform spawnPoint;
    private HumanController _humanController;
    
    private void Start()
    {
        _human = Instantiate(humanPrefab, spawnPoint.position, Quaternion.identity); // 스폰 위치에 생성
        _humanController = _human.GetComponent<HumanController>();
    }

    private void Update()
    {
        #region TestCode

        // 스페이스바 누르면 웨이브 시작
        if (Input.GetKeyDown("space"))
        {
            _human.IsWaveStarted = true;
        }

        // A키 누르면 몬스터에게 인간이 공격을 당한 것(깜짝 놀란 것)으로 간주
        if (Input.GetKeyDown(KeyCode.A))
        {
            _human.targetMonster = FindObjectOfType<Monster>();
            if (_human.targetMonster == null)
                Debug.LogAssertion("TargetMonster is null");
            // 몬스터의 시작 피로도 임의로 설정(디폴트 값도 0임)
            //_human.targetMonster.CurrentFatigue = 0;
            _humanController.ReactToScaring();
        }

        #endregion
    }
}
