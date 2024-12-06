using TMPro;
using UnityEngine;

public class StageInfoController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private TextMeshProUGUI waveTxt;

    private StageManager stageManager;

    private void Awake()
    {
        stageManager = StageManager.Instance;
    }

    // UI 변경(웨이브, 체력, 골드)
    public void ChangeUI()
    {
        waveTxt.text = $"Wave {stageManager.CurrWave} / {stageManager.totalWave}";
        healthTxt.text = stageManager.CurrHealth.ToString();
        goldTxt.text = stageManager.CurrGold.ToString();
    }
}
