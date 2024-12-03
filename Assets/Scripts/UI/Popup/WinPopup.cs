using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour
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
    }

    private void LoadGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StageScene");
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
        if (starsCount >= 3)
        {
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(false);
            emptyStar3.SetActive(false);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(true);
            FilledStar3.SetActive(true);
        }
        if (starsCount == 2)
        {
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(false);
            emptyStar3.SetActive(true);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(true);
            FilledStar3.SetActive(false);
        }
        if (starsCount == 1)
        {
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(true);
            emptyStar3.SetActive(true);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(false);
            FilledStar3.SetActive(false);
        }
        else { Debug.LogError("starsCount정보가 없습니다"); }
    }

}

