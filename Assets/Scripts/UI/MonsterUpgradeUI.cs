using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUpgradeUI : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI currentStatsText;
    [SerializeField] private TextMeshProUGUI upgradeStatsText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    private Monster selectedMonster;

    public void Show(Monster monster)
    {
        gameObject.SetActive(true);
        selectedMonster = monster;
        
        Vector3 worldPosition = selectedMonster.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Position the UI element at the screen position above the monster
        transform.position = screenPosition + new Vector3(0, 100, 0); // Adjust 100 as needed to position above the monster

        // Show current stats
        currentStatsText.text = $"Fatigue: {monster.data.fatigue}\n" +
                                $"Fear Inflicted: {monster.data.fearInflicted}\n" +
                                $"Cooldown: {monster.data.cooldown}";

        // Show upgrade stats
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

            gameObject.SetActive(false);
        }
        else
        {
            print("Not enough gold to upgrade!");
        }
    }
}