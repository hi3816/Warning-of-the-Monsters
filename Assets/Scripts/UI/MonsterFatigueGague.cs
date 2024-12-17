using UnityEngine;
using UnityEngine.UI;

public class MonsterFatigueGague : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Image MonsterFatigueGagueImg;
    
    private float _maxFatigue;
    
    private void Awake()
    {
        if (_monster == null)
        {
            _monster = gameObject.GetComponent<Monster>();
        }
        if (MonsterFatigueGagueImg == null)
        {
            MonsterFatigueGagueImg = gameObject.transform.Find("FatigueCanvas/FatigueGauge/Front").GetComponent<Image>();
        }
        _maxFatigue = _monster.data.fatigue;
    }

    private void OnEnable()
    {
        UpdateFatigueGauge();
        _monster.OnAttacked -= UpdateFatigueGauge;
        _monster.OnAttacked += UpdateFatigueGauge;
    }

    private void UpdateFatigueGauge()
    {
        MonsterFatigueGagueImg.fillAmount = _monster.data.currentFatigue / _maxFatigue;
    }
}
