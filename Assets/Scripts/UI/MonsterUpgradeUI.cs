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

    void UpdateUI()
    {
        Vector3 worldPosition = selectedMonster.transform.position;
        uiPanel.transform.position = worldPosition + new Vector3(0, 1, -1);

        var nextUpgrade = MonsterDataManager.Instance.GetUpgradeData(selectedMonster.data.id, selectedMonster.data.currentLevel + 1);
        if (nextUpgrade != null)
        {
            upgradeStatsText.text = $"Fatigue: {nextUpgrade.fatigue}\n" +
                                    $"Min Fear Inflicted: {nextUpgrade.minFearInflicted}\n" +
                                    $"Max Fear Inflicted: {nextUpgrade.maxFearInflicted}\n" + 
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
    
    public void Hide()
    {
        upgradeCanvas.gameObject.SetActive(false);
    }
}