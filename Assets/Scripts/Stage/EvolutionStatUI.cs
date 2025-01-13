using DataTable;
using TMPro;
using UnityEngine;

public class EvolutionStatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _evolutionNameText;

    [Header("CurrStat")]
    [SerializeField] private TextMeshProUGUI _fatigueText;
    [SerializeField] private TextMeshProUGUI _minFearInflictedText;
    [SerializeField] private TextMeshProUGUI _maxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    [Header("DiffStat")]
    [SerializeField] private TextMeshProUGUI _diffFatigueText;
    [SerializeField] private TextMeshProUGUI _diffMinFearInflictedText;
    [SerializeField] private TextMeshProUGUI _diffMaxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _diffCooldownText;

    public void Show(MonsterData currData, Evolution_Data evolution)
    {
        gameObject.SetActive(true);
        SetText(currData, evolution);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetText(MonsterData currData, Evolution_Data evolutionData)
    {

        _evolutionNameText.text = evolutionData.evolutionName;

        _fatigueText.text = currData.fatigue.ToString();
        _minFearInflictedText.text = currData.minFearInflicted.ToString();
        _maxFearInflictedText.text = currData.maxFearInflicted.ToString();
        _cooldownText.text = currData.cooldown.ToString();
        _diffFatigueText.text = CalcDiffValueToString(currData.fatigue, evolutionData.fatigue);
        _diffMinFearInflictedText.text = CalcDiffValueToString(currData.minFearInflicted, evolutionData.minFearInflicted);
        _diffMaxFearInflictedText.text = CalcDiffValueToString(currData.maxFearInflicted, evolutionData.maxFearInflicted);
        _diffCooldownText.text = CalcDiffValueToString(currData.cooldown, evolutionData.cooldown);
    }

    private string CalcDiffValueToString(float curr, float evolution)
    {
        float diff = Mathf.Round((evolution - curr) * 10f) / 10f;
        if (diff > 0)
        {
            return $"+ {diff.ToString()}";
        }
        else if (diff == 0)
        {
            return $"0";
        }
        else
        {
            return $"- {Mathf.Abs(diff).ToString()}";
        }
    }
}
