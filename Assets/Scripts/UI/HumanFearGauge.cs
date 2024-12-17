using UnityEngine;
using UnityEngine.UI;

public class HumanFearGauge : MonoBehaviour
{
    [SerializeField] private Human _human;
    [SerializeField] private Image HumanFearGagueImg;

    private float _maxFear = 0;
    
    private void Awake()
    {
        if (_human == null)
        {
            _human = gameObject.GetComponent<Human>();
        }
        if (HumanFearGagueImg == null)
        {
            HumanFearGagueImg = gameObject.transform.Find("Canvas/FearGauge/Front").GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (!Mathf.Approximately(_maxFear, 0f))
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
        HumanFearGagueImg.fillAmount = _human.FearLevel / _maxFear;
    }
}