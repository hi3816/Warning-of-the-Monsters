using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class MonsterUpgradeUI : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Canvas upgradeCanvas;
    [SerializeField] private TextMeshProUGUI upgradeStatsText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private GameObject rangeIndicator;
    public GameObject uiPanel;
    public Button upgradeButton;
    public Button sellButton;
    private Monster selectedMonster;
    
    public void Show(Monster monster)
    {
        selectedMonster = monster;
        upgradeCanvas.gameObject.SetActive(true);
        UpdateRangeIndicator();
        UpdateUI();
    }

    void UpdateUI()
    {
        Vector3 worldPosition = selectedMonster.transform.position;
        uiPanel.transform.position = worldPosition + new Vector3(0, 1, -1);

        var nextUpgrade = MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, selectedMonster.data.currentLevel + 1);
        if (nextUpgrade != null)
        {
            upgradeStatsText.text = $"Fatigue: \t{nextUpgrade.fatigue}\n" +
                                    $"Min Fear: \t{nextUpgrade.minFearInflicted}\n" +
                                    $"Max Fear: \t{nextUpgrade.maxFearInflicted}\n" + 
                                    $"Cooldown: \t{nextUpgrade.cooldown}";
            upgradeCostText.text = $"{nextUpgrade.requiredCoins}";
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeStatsText.text = "Max Upgrade Reached";
            upgradeCostText.text = "0";
            upgradeButton.interactable = false;
        }
    }
    
    public void UpgradeMonster()
    {
        if (selectedMonster == null) return;
        
        var nextUpgrade = MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, selectedMonster.data.currentLevel + 1);
        if (nextUpgrade != null && stageManager.CurrGold >= nextUpgrade.requiredCoins)
        {
            stageManager.ChangeGold(-nextUpgrade.requiredCoins);
            selectedMonster.Upgrade(nextUpgrade);
            if (MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, selectedMonster.data.currentLevel + 1) == null)
            {
                upgradeCanvas.gameObject.SetActive(false);
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
        if (selectedMonster == null || rangeIndicator == null) return;
        float range = selectedMonster.data.humanScaringRange;
        rangeIndicator.transform.localScale = new Vector3(range, range, 1);
        rangeIndicator.transform.position = selectedMonster.transform.position;
        rangeIndicator.gameObject.SetActive(true);
    }
    
    public void SellMonster()
    {
        if (selectedMonster == null) return;
        // upgradeButton.interactable = false;
        // sellButton.interactable = false;
        int totalSpent = CalculateTotalSpent(selectedMonster); //여태 얼마 사용했는지 계산
        float refundPercentage = 0.35f; // 35% 환불
        int refundAmount = Mathf.RoundToInt(totalSpent * refundPercentage);
        stageManager.ChangeGold(refundAmount); //UI에 표시
        selectedMonster.gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(selectedMonster.data.poolTag, selectedMonster.gameObject);
        // selectedMonster.ReturnToVillage();
        Hide();
    }

    private int CalculateTotalSpent(Monster selectedMonster) //몬스터 스폰 & 업그레이드에 사용한 비용 계산
    {
        int totalSpent = selectedMonster.data.requiredCoins; //몬스터 스폰 비용
        for (int level = 1; level <= selectedMonster.data.currentLevel; level++) //몬스터 업그레이드 비용
        { 
            var upgradeData = MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, level);
            if (upgradeData != null)
            {
                totalSpent += upgradeData.requiredCoins;
            }
        }
        return totalSpent;
    }
    
    public void Hide()
    {
        upgradeCanvas.gameObject.SetActive(false);
        if (selectedMonster != null && rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }
}