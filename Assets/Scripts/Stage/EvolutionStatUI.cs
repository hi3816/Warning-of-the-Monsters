using Monster_Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class EvolutionStatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI evolutionNameText;

    [SerializeField] private TextMeshProUGUI fatigueText;
    [SerializeField] private TextMeshProUGUI minFearInflictedText;
    [SerializeField] private TextMeshProUGUI maxFearInflictedText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI rangeText;

    public void Show(Evolution_Data evolution)
    {
        SetText(evolution);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetText(Evolution_Data evolutionData)
    {
        fatigueText.text = evolutionData.fatigue.ToString();
        minFearInflictedText.text = evolutionData.minFearInflicted.ToString();
        maxFearInflictedText.text = evolutionData.maxFearInflicted.ToString();
        cooldownText.text = evolutionData.cooldown.ToString();
        rangeText.text = evolutionData.humanScaringRange.ToString();
    }
}
