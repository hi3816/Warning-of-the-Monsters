using DataTable;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public interface ISell
{
    void SellMonster();
    int CalculateTotalSpent(Monster selectMonster);
}

public class MonsterUpgradeUI : MonoBehaviour, ISell
{
    [SerializeField] private StageManager _stageManager;
    [SerializeField] private GameObject _upgradeCanvas;
    [SerializeField] private GameObject _upgradeStat;
    [SerializeField] private GameObject _maxUpgradePanel;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private TextMeshProUGUI _sellButtonText;
    [SerializeField] private TextMeshProUGUI _nameText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _fatigueText;
    [SerializeField] private TextMeshProUGUI _minFearInflictedText;
    [SerializeField] private TextMeshProUGUI _maxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    [Header("DiffStat")]
    [SerializeField] private TextMeshProUGUI _diffFatigueText;
    [SerializeField] private TextMeshProUGUI _diffMinFearInflictedText;
    [SerializeField] private TextMeshProUGUI _diffMaxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _diffCooldownText;

    private Monster _selectedMonster;
    private MonsterUI _monsterUI;

    private void Awake()
    {
        _stageManager = StageManager.Instance;
        _monsterUI = GetComponentInParent<MonsterUI>();
    }

    public void Show(Monster monster)
    {
        _selectedMonster = monster;
        _upgradeCanvas.SetActive(true);
        UpdateUI();
    }

    void UpdateUI()
    {
        SetMonsterStatPosition();

        MonsterData data = _selectedMonster.data;
        _nameText.text = data.poolTag;
        int nextLevel = data.currentLevel + 1;
        var upgrade = DataManager.Instance.GetBaseMonsterById(data.id);
        if (data.currentLevel < data.maxLevel)
        {
            var nextUpgrade = upgrade;

            _fatigueText.text = $"{data.fatigue}";
            _minFearInflictedText.text = $"{data.minFearInflicted}";
            _maxFearInflictedText.text = $"{data.maxFearInflicted}"; 
            _cooldownText.text = $"{data.cooldown}";
            _diffFatigueText.text = CalcDiffValueToString(data.fatigue, nextUpgrade.fatigue[nextLevel]);
            _diffMinFearInflictedText.text = CalcDiffValueToString(data.minFearInflicted, nextUpgrade.minFearInflicted[nextLevel]);
            _diffMaxFearInflictedText.text = CalcDiffValueToString(data.maxFearInflicted, nextUpgrade.maxFearInflicted[nextLevel]);
            _diffCooldownText.text = CalcDiffValueToString(data.cooldown, nextUpgrade.cooldown[nextLevel]);
            _upgradeCostText.text = $"{nextUpgrade.requiredCoins[nextLevel]}";
            _upgradeButton.interactable = true;
            _upgradeStat.SetActive(true);
            _maxUpgradePanel.SetActive(false);
        }
        else
        {
            _upgradeCostText.text = "0";
            _upgradeButton.interactable = false;
            _upgradeStat.SetActive(false);
            _maxUpgradePanel.SetActive(true);
        }

        _sellButtonText.text = Mathf.RoundToInt(CalculateTotalSpent(_selectedMonster) * 0.35f).ToString();
    }

    public void UpgradeMonster()
    {
        if (_selectedMonster == null) return;

        int nextLevel = _selectedMonster.data.currentLevel + 1;
        var upgrade = DataManager.Instance.GetBaseMonsterById(_selectedMonster.data.id);
        if (upgrade.fatigue.Count < nextLevel) return;
        if (upgrade.maxLevel > 0 && _stageManager.CurrGold >= upgrade.requiredCoins[nextLevel])
        {
            _stageManager.ChangeGold(-upgrade.requiredCoins[nextLevel]);
            _selectedMonster.Upgrade(upgrade);
            if (upgrade.maxLevel <= _selectedMonster.data.currentLevel + 1)
            {
                _upgradeCanvas.SetActive(false);
            }
            else
            {
                UpdateUI();
            }
            _monsterUI.ShowRangeIndicator();
        }
        else
        {
            //print("Not enough gold to upgrade!");
        }
    }
    
    public void SellMonster()
    {
        if (_selectedMonster == null) return;
        int totalSpent = CalculateTotalSpent(_selectedMonster); //여태 얼마 사용했는지 계산
        float refundPercentage = 0.35f; // 35% 환불
        int refundAmount = Mathf.RoundToInt(totalSpent * refundPercentage);
        _stageManager.ChangeGold(refundAmount); //UI에 표시
        _selectedMonster.ReturnToVillage();
        _selectedMonster.gameObject.SetActive(false);
        Hide();
    }

    public int CalculateTotalSpent(Monster selectedMonster) //몬스터 스폰 & 업그레이드에 사용한 비용 계산
    {
        MonsterData monsterData = selectedMonster.data;
        List<int> goldList = DataManager.Instance.GetBaseMonsterById(monsterData.id).requiredCoins;
        int totalSpent = 0;
        int maxLevelToCalculate = monsterData.currentLevel < monsterData.maxLevel ? monsterData.currentLevel : monsterData.maxLevel - 1;
        for (int level = 0; level <= maxLevelToCalculate; level++) //몬스터 업그레이드 비용
        {
            totalSpent += goldList[level];
        }
        if (monsterData.currentLevel == monsterData.maxLevel)
        {
            Evolution_Data evolution = DataManager.Instance.GetEvolutionData(monsterData.id, monsterData.currentLevel, monsterData.evolutionType);
            totalSpent += evolution.requiredCoins;
        }
        return totalSpent;
    }
    
    public void Hide()
    {
        _upgradeCanvas.SetActive(false);
        _selectedMonster = null;
        if(_monsterUI != null && _monsterUI.gameObject.activeSelf) _monsterUI.HideRangeIndicator();
    }

    private string CalcDiffValueToString(float curr, float upgrade)
    {
        float diff = Mathf.Round((upgrade - curr) * 10f) / 10f;
        if (diff > 0)
        {
            return $"+ {diff.ToString()}";
        }
        else if (diff == 0)
        {
            return $"0";
        }
        else
        {
            return $"- {Mathf.Abs(diff).ToString()}";
        }
    }

    private void SetMonsterStatPosition()
    {
        Vector3 posX = _selectedMonster.transform.position.x > 0 ? Vector3.left : Vector3.right;
        _upgradeCanvas.transform.position = _selectedMonster.transform.position + (posX * 1.75f);
    }
}