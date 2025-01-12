using System;
using System.Collections.Generic;
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

    private int _timeScaleListIdx;
    private List<float> _timeScaleList = new List<float>();

    private void Awake()
    {
        _speedButton.onClick.AddListener(ClickSpeedButton);
        _timeScaleList.Add(1f);
        _timeScaleList.Add(1.5f);
        _timeScaleList.Add(2.0f);
    }

    private void OnEnable()
    {
        _timeScaleListIdx = 0;
        Time.timeScale = _timeScaleList[_timeScaleListIdx];
    }

    private void ClickSpeedButton()
    {
        // 1, 2, 3 돌리기
        _timeScaleListIdx = (_timeScaleListIdx == 2) ? 0 : ++_timeScaleListIdx;
        Time.timeScale = _timeScaleList[_timeScaleListIdx];
        ChangeButton();
    }

    public void ChangeButton()
    {
        _speedButtonText.text = $"x {_timeScaleList[_timeScaleListIdx]}";

        switch (_timeScaleListIdx)
        {
            case 0:
                _speedButtonImage.sprite = _disableSprite;
                break;
            case 1:
                _speedButtonImage.sprite = _enableSprite;
                break;
            case 2:
                _speedButtonImage.sprite = _enableSprite;
                break;
        }
    }
}
