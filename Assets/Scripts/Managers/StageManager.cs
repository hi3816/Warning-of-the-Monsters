using JetBrains.Annotations;
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
    [SerializeField] private int totalWave; // 총 웨이브
    [SerializeField] private int currWave; // 현재 웨이브
    [SerializeField] private int currHealth; // 현재 체력
    [SerializeField] private int currGold; // 현재 골드

    private SoundManager soundManager;

    [Header("Stage")]
    [SerializeField] private GameObject[] stages;
    [SerializeField] private GameObject stage;
    [SerializeField] private int stageIdx;
    [SerializeField] private StartBattleButtonController startBattleBtnController;

    protected override void Awake()
    {
        base.Awake();
        stage = Instantiate<GameObject>(stages[stageIdx]);
        startBattleBtnController = stage.GetComponentInChildren<StartBattleButtonController>();
        soundManager = SoundManager.Instance;
        SetStageStat();
        ChangeUI();
    }

    private void Start()
    {
        soundManager.PlayBGM(BgmType.Stage);
    }

    // stageSO를 통해 기본 값 초기화
    private void SetStageStat()
    {
        totalWave = stageSO.wave;
        currWave = 0;
        currHealth = stageSO.health;
        currGold = stageSO.gold;
    }

    // 현재 값들로 UI 조정
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
        // 웨이브 증가 후 텍스트 변경
        if (currWave >= totalWave) return;
        currWave++;
        ChangeUI();
    }

    // 현재 스테이지 클리어 됐는지 체크
    public bool CheckEndStage()
    {
        return (currWave == totalWave);
    }

    // Stop 버튼 눌렀을 때 optionPanel 활성화
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // BGM 버튼 클릭 시 On Off
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

    // SFX 버튼 클릭 시 On Off
    public void ClickSfxButton()
    {
        soundManager.ChangeIsPlaySFX();
    }

    // Retry 버튼 클릭 시 optionPanel 비활성화
    public void ClickRetryButton()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageScene");
    }

    public void ClickEndWaveBtn()
    {
        startBattleBtnController.EndWave();
    }
}
