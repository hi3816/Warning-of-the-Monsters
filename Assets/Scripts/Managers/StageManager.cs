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
    [SerializeField] private int totalWave; // �� ���̺�
    [SerializeField] private int currWave; // ���� ���̺�
    [SerializeField] private int currHealth; // ���� ü��
    public float currGold; // ���� ���

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

    // stageSO�� ���� �⺻ �� �ʱ�ȭ
    private void SetStageStat()
    {
        stage = Instantiate<GameObject>(stages[stageIdx]);
        startBattleBtnController = stage.GetComponentInChildren<StartBattleButtonController>();
        totalWave = stageSO.wave;
        currWave = 0;
        currHealth = stageSO.health;
        currGold = stageSO.gold;
    }

    // ���� ����� UI ����
    private void ChangeUI()
    {
        waveTxt.text = $"Wave {currWave} / {totalWave}";
        healthTxt.text = currHealth.ToString();
        goldTxt.text = currGold.ToString();
    }

    // health ����
    public void ChangeHealth(int health)
    {
        currHealth += health;
        ChangeUI();
    }

    // Wave ������Ʈ
    public void UpdateWave()
    {
        // ���̺� ���� �� �ؽ�Ʈ ����
        if (currWave >= totalWave) return;
        currWave++;
        ChangeUI();
    }

    // ���� �������� Ŭ���� �ƴ��� üũ
    public bool CheckEndStage()
    {
        return (currWave == totalWave);
    }

    // Stop ��ư ������ �� optionPanel Ȱ��ȭ
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // BGM ��ư Ŭ�� �� On Off
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

    // SFX ��ư Ŭ�� �� On Off
    public void ClickSfxButton()
    {
        soundManager.ChangeIsPlaySFX();
    }

    // Retry ��ư Ŭ�� �� optionPanel ��Ȱ��ȭ
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
