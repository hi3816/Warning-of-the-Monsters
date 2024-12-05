using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private int currWave; // 현재 웨이브
    [SerializeField] private int currHealth; // 현재 체력
    public float currGold; // 현재 골드

    [Header("Stage")]

    [SerializeField] private GameObject stage;
    [SerializeField] private int stageIdx;
    [SerializeField] private StartBattleButtonController startBattleBtnController;

    [Header("ToggleButton")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle; 

    private SoundManager soundManager;

    protected override void Awake()
    {
        base.Awake();
        soundManager = SoundManager.Instance;
        SetStageStat();
        SetStageObject();
        SetToggleIsOn();
        ChangeUI();
    }

    private void Start()
    {
        soundManager.PlayBGM(BgmType.Stage);
    }

    // stage에 대한 정보 초기화
    private void SetStageStat()
    {
        // 현재 Stage의 Stat 설정
        stageSO = DataManager.Instance.GetStageByIndex(stageIdx);
        totalWave = stageSO.wave;
        currWave = 0;
        currHealth = stageSO.health;
        currGold = stageSO.gold;
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

    // ToggleButton 초기화
    private void SetToggleIsOn()
    {
        if (bgmToggle != null)
        {
            bgmToggle.isOn = soundManager.IsPlayBGM;
            
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = soundManager.IsPlaySFX; 

        }
        
    }

    // UI 변경(웨이브, 체력, 골드)
    private void ChangeUI()
    {
        waveTxt.text = $"Wave {currWave} / {totalWave}";
        healthTxt.text = currHealth.ToString();
        goldTxt.text = currGold.ToString();
    }

    // health 변경
    public void ChangeHealth(int health)
    {
        currHealth += health;
        ChangeUI();
    }

    // Wave 업데이트
    public void UpdateWave()
    {
        // 전체 웨이브가 끝나면 return
        if (currWave >= totalWave) return;
        currWave++;
        ChangeUI();
    }

    // 현재 스테이지의 모든 웨이브가 끝났는지 확인
    public bool CheckEndStage()
    {
        return (currWave == totalWave);
    }

    // StopPanel 활성화
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Retry 버튼 클릭
    public void ClickRetryButton()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageScene");
    }

    // 테스트 버튼 클릭
    public void ClickEndWaveBtn()
    {
        startBattleBtnController.EndWave();
    }
}
