using TMPro;
using UnityEngine;

public class StageInfoController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthTxt;
    [SerializeField] private TextMeshProUGUI _goldTxt;
    [SerializeField] private TextMeshProUGUI _waveTxt;

    private StageManager _stageManager;

    private void Awake()
    {
        _stageManager = StageManager.Instance;
    }

    // UI 변경(웨이브, 체력, 골드)
    public void ChangeUI()
    {
        _waveTxt.text = $"Wave {_stageManager.CurrWave} / {_stageManager.TotalWave}";
        _healthTxt.text = _stageManager.CurrHealth.ToString();
        _goldTxt.text = _stageManager.CurrGold.ToString();
    }
}
