using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButton : MonoBehaviour
{
    [SerializeField] private Button speedButton2x;
    [SerializeField] private Button speedButton3x;

    private List<Button> speedButtons = new List<Button>();

    private enum TimeScale
    {
        OneX = 1,
        TwoX = 2,
        ThreeX = 3,
    }

    private void Awake()
    {
        speedButton2x.onClick.AddListener(() => ClickSpeedButton((int)TimeScale.TwoX));
        speedButton3x.onClick.AddListener(() => ClickSpeedButton((int)TimeScale.ThreeX));
        speedButtons.Add(speedButton2x);
        speedButtons.Add(speedButton3x);
    }

    private void OnEnable()
    {
        Time.timeScale = 1.0f;
    }

    private void ClickSpeedButton(int speed)
    {
        Time.timeScale = speed;
    }
}
