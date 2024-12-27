using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

public class MonsterUpgradeUI : MonoBehaviour
{
    [SerializeField] private StageManager _stageManager;
    [SerializeField] private Canvas _upgradeCanvas;
    [SerializeField] private TextMeshProUGUI _upgradeStatsText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private GameObject _rangeIndicator;
    public GameObject UiPanel;
    public Button UpgradeButton;
    public Button SellButton;
    private Monster _selectedMonster;
    
    public void Show(Monster monster)
    {
        _selectedMonster = monster;
        _upgradeCanvas.gameObject.SetActive(true);
        UpdateRangeIndicator();
        UpdateUI();
    }

    void UpdateUI()
    {
        Vector3 worldPosition = _selectedMonster.transform.position;
        UiPanel.transform.position = worldPosition + new Vector3(0, 1, -1);

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
                UpdateRangeIndicator();
            }
        }
        else
        {
            print("Not enough gold to upgrade!");
        }
    }
    
    private void UpdateRangeIndicator()
    {
        if (_selectedMonster == null || _rangeIndicator == null) return;
        float range = _selectedMonster.data.humanScaringRange;
        _rangeIndicator.transform.localScale = new Vector3(range, range, 1);
        _rangeIndicator.transform.position = _selectedMonster.transform.position;
        _rangeIndicator.gameObject.SetActive(true);
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
        PoolManager.Instance.ReturnToPool(_selectedMonster.data.poolTag, _selectedMonster.gameObject);
        // selectedMonster.ReturnToVillage();
        Hide();
    }

    private int CalculateTotalSpent(Monster selectedMonster) //몬스터 스폰 & 업그레이드에 사용한 비용 계산
    {
        int totalSpent = selectedMonster.data.requiredCoins; //몬스터 스폰 비용
        for (int level = 1; level <= selectedMonster.data.currentLevel; level++) //몬스터 업그레이드 비용
        { 
            var upgrades = DataManager.Instance.GetUpgradeMonsters(selectedMonster.data.id, level);
            if (upgrades.upgradeLevel > 0)
            {
                var upgradeData = upgrades;
                totalSpent += upgradeData.requiredCoins;
            }
        }
        return totalSpent;
    }
    
    public void Hide()
    {
        _upgradeCanvas.gameObject.SetActive(false);
        if (_selectedMonster != null && _rangeIndicator != null)
        {
            _rangeIndicator.SetActive(false);
        }
    }
}