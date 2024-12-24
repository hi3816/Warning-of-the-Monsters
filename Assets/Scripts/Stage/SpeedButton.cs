using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButton : MonoBehaviour
{
    [SerializeField] private Button _speedButton;
    [SerializeField] private Image _speedButtonImage;
    [SerializeField] private TextMeshProUGUI _speedButtonText;
    [SerializeField] private Sprite _enableSprite;
    [SerializeField] private Sprite _disableSprite;

    private int _timeScale;

    private void Awake()
    {
        _speedButton.onClick.AddListener(() => ClickSpeedButton());
    }

    private void OnEnable()
    {
        _timeScale = 1;
        Time.timeScale = (float)_timeScale;
    }

    private void ClickSpeedButton()
    {
        // 1, 2, 3 돌리기
        _timeScale = (_timeScale == 3) ? 1 : ++_timeScale;
        Time.timeScale = (float)_timeScale;
        ChangeButton();
    }

    public void ChangeButton()
    {
        _speedButtonText.text = $"x {_timeScale}";

        switch (_timeScale)
        {
            case 1:
                _speedButtonImage.sprite = _disableSprite;
                break;
            case 2:
                _speedButtonImage.sprite = _enableSprite;
                break;
            case 3:
                _speedButtonImage.sprite = _enableSprite;
                break;
        }
    }
}
