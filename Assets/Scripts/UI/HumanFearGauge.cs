using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HumanFearGauge : MonoBehaviour
{
    [SerializeField] private Human human;
    [SerializeField] private Image HumanFearGagueImg;

    private float _maxFear = 0;
    
    private void Awake()
    {
        if (human == null)
        {
            human = gameObject.GetComponent<Human>();
        }
        if (HumanFearGagueImg == null)
        {
            HumanFearGagueImg = gameObject.transform.Find("Canvas/FearGauge/Front").GetComponent<Image>();
        }
        _maxFear = human.MaxFear;
    }

    private void OnEnable()
    {
        if (!Mathf.Approximately(_maxFear, 0f)) // 최대 공포 수치 초기화됐는지 확인
            UpdateFearGauge();
        human.OnAttacked -= UpdateFearGauge;
        human.OnAttacked += UpdateFearGauge;
    }

    private void Start()
    {
        _maxFear = human.MaxFear;
        UpdateFearGauge();
    }
    
    private void UpdateFearGauge()
    {
        HumanFearGagueImg.fillAmount = human.FearLevel / _maxFear;
    }
}