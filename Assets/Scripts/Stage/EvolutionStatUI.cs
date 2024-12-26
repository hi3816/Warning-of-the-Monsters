using DataTable;
using TMPro;
using UnityEngine;

public class EvolutionStatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _evolutionNameText;

    [SerializeField] private TextMeshProUGUI _fatigueText;
    [SerializeField] private TextMeshProUGUI _minFearInflictedText;
    [SerializeField] private TextMeshProUGUI _maxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private TextMeshProUGUI _rangeText;

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
        _fatigueText.text = evolutionData.fatigue.ToString();
        _minFearInflictedText.text = evolutionData.minFearInflicted.ToString();
        _maxFearInflictedText.text = evolutionData.maxFearInflicted.ToString();
        _cooldownText.text = evolutionData.cooldown.ToString();
        _rangeText.text = evolutionData.humanScaringRange.ToString();
    }
}
