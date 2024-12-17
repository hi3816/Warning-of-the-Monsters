using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MonsterUpgradeUI : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Canvas upgradeCanvas;
    [SerializeField] private TextMeshProUGUI upgradeStatsText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject RangeIndicator;
    public GameObject uiPanel;
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
            }
        }
        else
        {
            print("Not enough gold to upgrade!");
        }
    }
    
    private void UpdateRangeIndicator()
    {
        if (selectedMonster == null || RangeIndicator == null) return;
        RangeIndicator.gameObject.SetActive(true);
        float range = selectedMonster.data.humanScaringRange;
        selectedMonster.transform.localScale = new Vector3(range * 2, range * 2, 1);
    }
    
    public void Hide()
    {
        upgradeCanvas.gameObject.SetActive(false);
        if (selectedMonster != null && RangeIndicator != null)
        {
            RangeIndicator.SetActive(false);
        }
    }
}