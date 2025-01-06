using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MonsterFatigueGague : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Image _monsterFatigueGagueImg;
    
    private float _maxFatigue = 0;
    
    private void Awake()
    {
        if (_monster == null)
        {
            _monster = gameObject.GetComponent<Monster>();
        }
        if (_monsterFatigueGagueImg == null)
        {
            _monsterFatigueGagueImg = gameObject.transform.Find("FatigueCanvas/FatigueGauge/Front").GetComponent<Image>();
        }
        _maxFatigue = _monster.data.fatigue;
    }

    private void OnEnable()
    {
        if (!Mathf.Approximately(_maxFatigue, 0f))  // 최대 피로도 초기화됐는지 확인
            UpdateFatigueGauge();
        _monster.OnAttacked -= UpdateFatigueGauge;
        _monster.OnAttacked += UpdateFatigueGauge;
        _monsterFatigueGagueImg.fillAmount = 0;
    }
    
    private void Start()
    {
        _maxFatigue = _monster.data.fatigue;
        UpdateFatigueGauge();
    }

    private void UpdateFatigueGauge()
    {
        _monsterFatigueGagueImg.fillAmount = _monster.data.currentFatigue / _maxFatigue;
    }

    public void SetFatigue()
    {
        _maxFatigue = _monster.data.fatigue;
        _monsterFatigueGagueImg.fillAmount = _monster.data.currentFatigue;
        UpdateFatigueGauge();
    }
}
