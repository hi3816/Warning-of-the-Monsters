using UnityEngine;
using UnityEngine.UI;

public class HumanFearGauge : MonoBehaviour
{
    [SerializeField] private Human _human;
    [SerializeField] private Image HumanFearGagueImg;

    private float _maxFear;
    
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
        _maxFear = _human.MaxFear;
    }

    private void OnEnable()
    {
        UpdateFearGauge();
        _human.OnAttacked -= UpdateFearGauge;
        _human.OnAttacked += UpdateFearGauge;
    }

    private void UpdateFearGauge()
    {
        HumanFearGagueImg.fillAmount = _human.FearLevel / _maxFear;
    }
}