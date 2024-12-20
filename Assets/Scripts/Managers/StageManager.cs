using System;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class StageManager : SingletonBase<StageManager>
{
    [Header("UI")]

    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject stageInfo;
    [SerializeField] private StageInfoController stageInfoController;

    public StageSO stageSO { get; private set; }

    public int totalWave { get; private set; } // 전체 웨이브 수
    public int CurrWave { get; private set; } // 현재 웨이브
    public int CurrHealth { get; private set; } // 현재 체력
    public float CurrGold { get; private set; } // 현재 골드

    [Header("Stage")]

    [SerializeField] private GameObject stage;
    [SerializeField] private StartBattleButtonController startBattleBtnController;
    
    private int stageIdx;
    public Transform SpawnPoint { get; private set; }
    public Transform EndPoint { get; private set; }

    [Header("Sfx Pools")]
    [SerializeField] private PoolManager.PoolConfig[] poolConfigs;

    public Action OnGameOver;

    protected override void Awake()
    {
        base.Awake();
        PoolManager.Instance.AddPoolS(poolConfigs);
        SoundManager.Instance.PlayBGM(BgmType.Stage);
        SetStageInfo();
        SetStageObject();
        SetPointInfo();
    }

    private void OnEnable()
    {
        Time.timeScale = 1;
    }

    // stage에 대한 정보 초기화
    private void SetStageInfo()
    {
        // 현재 Stage의 Stat 설정
        stageIdx = DataManager.Instance.selectedStageIdx;
        stageSO = DataManager.Instance.GetStageByIndex(stageIdx);
        totalWave = stageSO.wave;
        CurrWave = 0;
        CurrHealth = stageSO.health;
        CurrGold = stageSO.gold;
        stageInfoController = stageInfo.GetComponent<StageInfoController>();
        stageInfoController.ChangeUI();
    }

    // Stage 및 하위 오브젝트 캐싱
    private void SetStageObject()
    {
        // 스테이지 동적 로드
        stage = Instantiate<GameObject>(Resources.Load<GameObject>($"Prefabs/Stage/Stage{stageIdx + 1}"));
        stage.name = $"Stage{stageIdx + 1}";

        // startBattleBtn에 interWaveDelay필드에 값 저장하기 위해 StageSO 세팅 후에 캐싱
        startBattleBtnController = stage.GetComponentInChildren<StartBattleButtonController>();
    }

    // 스테이지의 시작점과 종료지점 캐싱
    private void SetPointInfo()
    {
        SpawnPoint = GameObject.FindGameObjectWithTag("HumanSpawnPoint").transform;
        if (SpawnPoint == null)
            Debug.LogAssertion("Spawnpoint not found");
        EndPoint = GameObject.FindGameObjectWithTag("DestinationPoint").transform;
        if (EndPoint == null)
            Debug.LogAssertion("Endpoint not found");
    }

    // Wave 업데이트
    public void UpdateWave()
    {
        // 전체 웨이브가 끝나면 return
        if (CurrWave >= totalWave) return;
        CurrWave++;
        stageInfoController.ChangeUI();
    }

    // 현재 스테이지의 모든 웨이브가 끝났는지 확인
    public bool CheckEndStage()
    {
        return (CurrWave == totalWave);
    }

    // health 변경
    public void ChangeHealth(int health)
    {
        CurrHealth += health;
        stageInfoController.ChangeUI();
        if (CurrHealth <= 0)
        {
            CurrHealth = 0;            
            OnGameOver?.Invoke();
        }
    }

    public void ChangeGold(int gold)
    {
        CurrGold += gold;
        stageInfoController.ChangeUI();
    }

    // StopPanel 활성화
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 테스트 버튼 클릭
    public void ClickEndWaveBtn()
    {
        startBattleBtnController.EndWave();
    }
}
