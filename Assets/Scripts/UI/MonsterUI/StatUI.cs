using DataTable;
using TMPro;
using UnityEngine;

public class StatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _monsterName;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI _fatigueText;
    [SerializeField] private TextMeshProUGUI _minFearInflictedText;
    [SerializeField] private TextMeshProUGUI _maxFearInflictedText;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    public void Show(Vector3 monsterPos, Monster_Data monsterData)
    {
        SetMonsterStatPosition(monsterPos);
        SetText(monsterData, 0);
    }

    private void SetMonsterStatPosition(Vector3 monsterPos)
    {
        Vector3 posX = monsterPos.x > 0 ? Vector3.left : Vector3.right;
        transform.position = monsterPos + (posX * 2.5f);
    }

    private void SetText(Monster_Data data, int index)
    {
        _monsterName.text = data.name;

        _fatigueText.text = data.fatigue[index].ToString();
        _minFearInflictedText.text = data.minFearInflicted[index].ToString();
        _maxFearInflictedText.text = data.maxFearInflicted[index].ToString();
        _cooldownText.text = data.cooldown[index].ToString();
    }
}
