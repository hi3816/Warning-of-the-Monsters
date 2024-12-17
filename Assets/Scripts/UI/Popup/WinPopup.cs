using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPopup : UIBase
{
    [Header("Button")]
    public Button btnRetry;
    public Button btnNextStage;

    [Header("Display")]
    public TextMeshProUGUI displayResultInfo;

    [Header("Star")]
    public GameObject emptyStar1;
    public GameObject emptyStar2;
    public GameObject emptyStar3;
    public GameObject FilledStar1;
    public GameObject FilledStar2;
    public GameObject FilledStar3;


    void Start()
    {
        btnRetry.onClick.AddListener(LoadGameScene);
        btnNextStage.onClick.AddListener(LoadNextStage);

        Debug.Log($"{StageManager.Instance.CurrHealth}");
        SetStars(StageManager.Instance.CurrHealth);
    }

    private void LoadGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
    private void LoadNextStage()
    {
        Debug.Log("다음 스테이지 로드");
    }

    private void ShowResultInfo()
    {
        displayResultInfo.text = "test";
    }

    public void SetStars(int starsCount)
    {
        if (starsCount >= 20)
        {
            //별 3개
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(false);
            emptyStar3.SetActive(false);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(true);
            FilledStar3.SetActive(true);
        }
        else if (starsCount > 10 && starsCount <= 20)
        {
            //별 2개
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(false);
            emptyStar3.SetActive(true);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(true);
            FilledStar3.SetActive(false);
        }
        else if (starsCount <= 10)
        {
            //별 1개
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(true);
            emptyStar3.SetActive(true);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(false);
            FilledStar3.SetActive(false);
        }
    }

}

