using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : SingletonBase<StageManager>
{
    [SerializeField] private StageSO stageSO;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI goldTxt;
 
    [SerializeField] private int totalWave; // 총 웨이브
    [SerializeField] private int currWave; // 현재 웨이브
    [SerializeField] private int currHealth; // 현재 체력
    [SerializeField] private int currGold; // 현재 골드

    private bool isStart = false;

    protected override void Awake()
    {
        base.Awake();

        SetStageStat();
        SetUI();
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
    private void SetUI()
    {
        waveTxt.text = $"Wave {currWave} / {totalWave}";
        healthTxt.text = currHealth.ToString();
        goldTxt.text = currGold.ToString();
    }

    // StartBattleBtn 클릭
    public void ClickStartBattleBtn()
    {
        isStart = true;
    }
}
