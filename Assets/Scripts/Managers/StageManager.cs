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

    [Header("Stat")]

    public StageSO stageSO;
    [SerializeField] private StageSO[] stageSOs;
    [SerializeField] private int totalWave; // 전체 웨이브 수
    [SerializeField] private int currWave; // 현재 웨이브
    [SerializeField] private int currHealth; // 현재 체력
    public float currGold; // 현재 골드

    private SoundManager soundManager;

    [Header("Stage")]
    [SerializeField] private GameObject[] stages;
    [SerializeField] private GameObject stage;
    [SerializeField] private int stageIdx;
    [SerializeField] private StartBattleButtonController startBattleBtnController;

    protected override void Awake()
    {
        base.Awake();
        soundManager = SoundManager.Instance;
        SetStageStat();
        ChangeUI();
    }

    private void Start()
    {
        soundManager.PlayBGM(BgmType.Stage);
    }

    // stage에 대한 정보 초기화
    private void SetStageStat()
    {
        stage = Instantiate<GameObject>(stages[stageIdx]);
        startBattleBtnController = stage.GetComponentInChildren<StartBattleButtonController>();
        totalWave = stageSO.wave;
        currWave = 0;
        currHealth = stageSO.health;
        currGold = stageSO.gold;
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

    // BGM 버튼 클릭 (On / Off)
    public void ClickBgmButton()
    {
        soundManager.ChangeIsPlayBGM();

        if (soundManager.IsPlayBGM)
        {
            soundManager.PlayBGM(BgmType.Stage);
        }
        else
        {
            soundManager.StopBGM();
        }
    }

    // SFX 버튼 클릭 (On / Off)
    public void ClickSfxButton()
    {
        soundManager.ChangeIsPlaySFX();
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
