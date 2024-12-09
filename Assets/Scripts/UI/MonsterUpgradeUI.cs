using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUpgradeUI : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Canvas upgradeCanvas;
    [SerializeField] private TextMeshProUGUI upgradeStatsText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    public GameObject uiPanel;
    private Monster selectedMonster;

    public void Show(Monster monster)
    {
        selectedMonster = monster;
        upgradeCanvas.gameObject.SetActive(true);
        UpdateUI();
    }
    
    public void OnUpgradeButtonClick()
    {
        if (selectedMonster != null)
        { 
            UpgradeMonster();
        }
    }

    void UpdateUI()
    {
        Vector3 worldPosition = selectedMonster.transform.position;
        uiPanel.transform.position = worldPosition + new Vector3(0, 1, -1);

        var nextUpgrade = MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, selectedMonster.currentUpgradeLevel + 1);
        if (nextUpgrade != null)
        {
            upgradeStatsText.text = $"Fatigue: {nextUpgrade.fatigue}\n" +
                                    $"Fear Inflicted: {nextUpgrade.fearInflicted}\n" +
                                    $"Cooldown: {nextUpgrade.cooldown}";
            upgradeCostText.text = $"Cost: {nextUpgrade.requiredCoins}";
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeStatsText.text = "Max Upgrade Reached";
            upgradeCostText.text = "";
            upgradeButton.interactable = false;
        }
    }
    
    public void UpgradeMonster()
    {
        if (selectedMonster == null) return;

        var nextUpgrade = MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, selectedMonster.currentUpgradeLevel + 1);
        if (nextUpgrade != null && stageManager.CurrGold >= nextUpgrade.requiredCoins)
        {
            stageManager.ChangeGold(-(int)nextUpgrade.requiredCoins);
            selectedMonster.Upgrade(nextUpgrade);
            upgradeCanvas.gameObject.SetActive(false);
        }
        else
        {
            print("Not enough gold to upgrade!");
        }
    }
    
    public void Hide()
    {
        upgradeCanvas.gameObject.SetActive(false);
    }
}