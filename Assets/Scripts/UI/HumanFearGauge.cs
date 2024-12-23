using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HumanFearGauge : MonoBehaviour
{
    [SerializeField] private Human _human;
    [SerializeField] private Image _humanFearGagueImg;

    private float _maxFear = 0;
    
    private void Awake()
    {
        if (_human == null)
        {
            _human = gameObject.GetComponent<Human>();
        }
        if (_humanFearGagueImg == null)
        {
            _humanFearGagueImg = gameObject.transform.Find("Canvas/FearGauge/Front").GetComponent<Image>();
        }
        _maxFear = _human.MaxFear;
    }

    private void OnEnable()
    {
        if (!Mathf.Approximately(_maxFear, 0f)) // 최대 공포 수치 초기화됐는지 확인
            UpdateFearGauge();
        _human.OnAttacked -= UpdateFearGauge;
        _human.OnAttacked += UpdateFearGauge;
    }

    private void Start()
    {
        _maxFear = _human.MaxFear;
        UpdateFearGauge();
    }
    
    private void UpdateFearGauge()
    {
        _humanFearGagueImg.fillAmount = _human.FearLevel / _maxFear;
    }
}