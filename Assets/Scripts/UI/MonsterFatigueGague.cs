using UnityEngine;
using UnityEngine.UI;

public class MonsterFatigueGague : MonoBehaviour
{
    [SerializeField] private Monster monster;
    [SerializeField] private Image MonsterFatigueGagueImg;
    
    private float _maxFatigue = 0;
    
    private void Awake()
    {
        if (monster == null)
        {
            monster = gameObject.GetComponent<Monster>();
        }
        if (MonsterFatigueGagueImg == null)
        {
            MonsterFatigueGagueImg = gameObject.transform.Find("FatigueCanvas/FatigueGauge/Front").GetComponent<Image>();
        }
        _maxFatigue = monster.data.fatigue;
    }

    private void OnEnable()
    {
        if (!Mathf.Approximately(_maxFatigue, 0f))  // 최대 피로도 초기화됐는지 확인
            UpdateFatigueGauge();
        monster.OnAttacked -= UpdateFatigueGauge;
        monster.OnAttacked += UpdateFatigueGauge;
    }
    
    private void Start()
    {
        _maxFatigue = monster.data.fatigue;
        UpdateFatigueGauge();
    }

    private void UpdateFatigueGauge()
    {
        MonsterFatigueGagueImg.fillAmount = monster.data.currentFatigue / _maxFatigue;
    }
}
