using TMPro;
using UnityEngine;

public class StageManager : SingletonBase<StageManager>
{
    [Header("UI")]

    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private GameObject optionPanel;

    public StageSO stageSO { get; private set; }

    [Header("Stat")]

    [SerializeField] private int totalWave; // 전체 웨이브 수
    [SerializeField] public int CurrWave { get; private set; } // 현재 웨이브
    [SerializeField] public int CurrHealth { get; private set; } // 현재 체력
    [SerializeField] public float CurrGold { get; private set; } // 현재 골드

    [Header("Stage")]

    [SerializeField] private GameObject stage;
    [SerializeField] private int stageIdx;
    [SerializeField] private StartBattleButtonController startBattleBtnController;
    
    private SoundManager soundManager;

    protected override void Awake()
    {
        base.Awake();
        soundManager = SoundManager.Instance;
        soundManager.PlayBGM(BgmType.Stage);
        SetStageStat();
        SetStageObject();
        ChangeUI();
    }

    // stage에 대한 정보 초기화
    private void SetStageStat()
    {
        // 현재 Stage의 Stat 설정
        stageSO = DataManager.Instance.GetStageByIndex(stageIdx);
        totalWave = stageSO.wave;
        CurrWave = 0;
        CurrHealth = stageSO.health;
        CurrGold = stageSO.gold;
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

    // UI 변경(웨이브, 체력, 골드)
    private void ChangeUI()
    {
        waveTxt.text = $"Wave {CurrWave} / {totalWave}";
        healthTxt.text = CurrHealth.ToString();
        goldTxt.text = CurrGold.ToString();
    }

    // Wave 업데이트
    public void UpdateWave()
    {
        // 전체 웨이브가 끝나면 return
        if (CurrWave >= totalWave) return;
        CurrWave++;
        ChangeUI();
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
        ChangeUI();
    }

    public void ChangeGold(int gold)
    {
        CurrGold += gold;
        ChangeUI();
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
