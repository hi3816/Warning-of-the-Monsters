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
    [SerializeField] private Canvas _upgradeCanvas;
    [SerializeField] private GameObject _upgradeStat;
    [SerializeField] private GameObject _maxUpgradePanel;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private TextMeshProUGUI _sellButtonText;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _fatigueText;
    [SerializeField] private TextMeshProUGUI _minFearInflictedText;
    [SerializeField] private TextMeshProUGUI _maxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    public GameObject UiPanel;
    public Button UpgradeButton;
    public Button SellButton;
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
        _upgradeCanvas.gameObject.SetActive(true);
        UpdateUI();
        _monsterUI.ShowRangeIndicator();
    }

    void UpdateUI()
    {
        Vector3 worldPosition = _selectedMonster.transform.position;
        UiPanel.transform.position = worldPosition + (Vector3.right * 1.5f);

        int nextLevel = _selectedMonster.data.currentLevel + 1;
        var upgrade = DataManager.Instance.GetBaseMonsterById(_selectedMonster.data.id);
        if (_selectedMonster.data.currentLevel < _selectedMonster.data.maxLevel)
        {
            var nextUpgrade = upgrade;

            _fatigueText.text = $"{nextUpgrade.fatigue[nextLevel]}";
            _minFearInflictedText.text = $"{nextUpgrade.minFearInflicted[nextLevel]}";
            _maxFearInflictedText.text = $"{nextUpgrade.maxFearInflicted[nextLevel]}"; 
            _cooldownText.text = $"{nextUpgrade.cooldown[nextLevel]}";
            _upgradeCostText.text = $"{nextUpgrade.requiredCoins[nextLevel]}";
            UpgradeButton.interactable = true;
            _upgradeStat.SetActive(true);
            _maxUpgradePanel.SetActive(false);
        }
        else
        {
            _upgradeCostText.text = "0";
            UpgradeButton.interactable = false;
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
                _upgradeCanvas.gameObject.SetActive(false);
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
        // upgradeButton.interactable = false;
        // sellButton.interactable = false;
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
        _upgradeCanvas.gameObject.SetActive(false);
        if(_monsterUI != null && _monsterUI.gameObject.activeSelf) _monsterUI.HideRangeIndicator();
    }
}