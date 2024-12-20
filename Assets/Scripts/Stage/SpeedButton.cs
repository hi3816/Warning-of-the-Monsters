using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButton : MonoBehaviour
{
    [SerializeField] private Button speedButton;
    [SerializeField] private Image speedButtonImage;
    [SerializeField] private TextMeshProUGUI speedButtonText;
    [SerializeField] private Sprite EnableSprite;
    [SerializeField] private Sprite DisableSprite;

    private List<Button> speedButtons = new List<Button>();
    private int timeScale;

    private void Awake()
    {
        speedButton.onClick.AddListener(() => ClickSpeedButton());
        speedButtons.Add(speedButton);
    }

    private void OnEnable()
    {
        timeScale = 1;
        Time.timeScale = 1.0f;
    }

    private void ClickSpeedButton()
    {
        // 1, 2, 3 돌리기
        timeScale = (timeScale == 3) ? 1 : ++timeScale;
        Time.timeScale = (float)timeScale;
        ChangeButton();
    }

    public void ChangeButton()
    {
        speedButtonText.text = $"x {timeScale}";

        switch (timeScale)
        {
            case 1:
                speedButtonImage.sprite = DisableSprite;
                break;
            case 2:
                speedButtonImage.sprite = EnableSprite;
                break;
            case 3:
                speedButtonImage.sprite = EnableSprite;
                break;
        }
    }
}
