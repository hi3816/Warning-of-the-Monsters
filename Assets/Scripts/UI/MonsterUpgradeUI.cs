using DataTable;
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
    [SerializeField] private TextMeshProUGUI _upgradeStatsText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private TextMeshProUGUI _sellButtonText;
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
        UiPanel.transform.position = worldPosition + (Vector3.up * 1.5f);

        var upgrades = DataManager.Instance.GetUpgradeMonsters(_selectedMonster.data.id, _selectedMonster.data.currentLevel + 1);
        if (upgrades != null)
        {
            var nextUpgrade = upgrades;
            
            _upgradeStatsText.text = $"Fatigue: \t{nextUpgrade.fatigue}\n" +
                                    $"Min Fear: \t{nextUpgrade.minFearInflicted}\n" +
                                    $"Max Fear: \t{nextUpgrade.maxFearInflicted}\n" + 
                                    $"Cooldown: \t{nextUpgrade.cooldown}";
            _upgradeCostText.text = $"{nextUpgrade.requiredCoins}";
            UpgradeButton.interactable = true;
        }
        else
        {
            _upgradeStatsText.text = "Max Upgrade Reached";
            _upgradeCostText.text = "0";
            UpgradeButton.interactable = false;
        }

        _sellButtonText.text = Mathf.RoundToInt(CalculateTotalSpent(_selectedMonster) * 0.35f).ToString();
    }

    public void UpgradeMonster()
    {
        if (_selectedMonster == null) return;
        
        var upgrades = DataManager.Instance.GetUpgradeMonsters(_selectedMonster.data.id, _selectedMonster.data.currentLevel + 1);
        if (upgrades.upgradeLevel > 0 && _stageManager.CurrGold >= upgrades.requiredCoins)
        {
            var nextUpgrade = upgrades;
            _stageManager.ChangeGold(-nextUpgrade.requiredCoins);
            _selectedMonster.Upgrade(nextUpgrade);
            if (DataManager.Instance.GetUpgradeMonsters(_selectedMonster.data.id, _selectedMonster.data.currentLevel + 1) == null)
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
            print("Not enough gold to upgrade!");
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
        _selectedMonster.gameObject.SetActive(false);
        if (_selectedMonster.data.monsterType == MonsterType.Summoner)
        {
            _selectedMonster.ReturnToVillage();
        }
        PoolManager.Instance.ReturnToPool(_selectedMonster.data.poolTag, _selectedMonster.gameObject);
        // selectedMonster.ReturnToVillage();
        Hide();
    }

    public int CalculateTotalSpent(Monster selectedMonster) //몬스터 스폰 & 업그레이드에 사용한 비용 계산
    {
        MonsterData monsterData = selectedMonster.data;
        int totalSpent = DataManager.Instance.GetBaseMonsterById(monsterData.id).requiredCoins; //몬스터 스폰 비용
        for (int level = 1; level <= monsterData.currentLevel; level++) //몬스터 업그레이드 비용
        {
            var upgrades = DataManager.Instance.GetUpgradeMonsters(monsterData.id, level);
            if (upgrades == null) continue;
            if (upgrades.upgradeLevel > 0)
            {
                var upgradeData = upgrades;
                totalSpent += upgradeData.requiredCoins;
            }
        }
        if(monsterData.currentLevel == monsterData.maxLevel)
        {
            Evolution_Data evolution = DataManager.Instance.GetEvolutionData(monsterData.id, monsterData.currentLevel, monsterData.evolutionType);
            totalSpent += evolution.requiredCoins;
        }
        return totalSpent;
    }
    
    public void Hide()
    {
        _upgradeCanvas.gameObject.SetActive(false);
        _monsterUI.HideRangeIndicator();
    }
}