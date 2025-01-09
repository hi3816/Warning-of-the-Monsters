using DG.Tweening;
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
    [SerializeField] RectTransform _winPopup;
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

        //SetStars(StageManager.Instance.CurrHealth);
        SetStars(StageManager.Instance.StarsCount);

        SetSortOrder(UILayerOrder.Popup);
    }

    private void OnEnable()
    {
        // 초기화
        _winPopup.localScale = Vector3.zero;

        //시간 재개. 없으면 애니메이션이 실행안됨.
        Time.timeScale = 1f;
        // 애니메이션
        _winPopup.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack);
    }

    private void LoadGameScene()
    {
        LoadingManager.Instance.ChangeScene("MainScene");
    }
    private void LoadNextStage()
    {
        //Debug.Log("다음 스테이지 로드");
    }

    private void ShowResultInfo()
    {
        displayResultInfo.text = "test";
    }

    public void SetStars(int starsCount)
    {
        if (starsCount == 3)
        {
            //별 3개
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(false);
            emptyStar3.SetActive(false);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(true);
            FilledStar3.SetActive(true);
        }
        else if (starsCount == 2)
        {
            //별 2개
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(false);
            emptyStar3.SetActive(true);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(true);
            FilledStar3.SetActive(false);
        }
        else if (starsCount == 1)
        {
            //별 1개
            emptyStar1.SetActive(false);
            emptyStar2.SetActive(true);
            emptyStar3.SetActive(true);

            FilledStar1.SetActive(true);
            FilledStar2.SetActive(false);
            FilledStar3.SetActive(false);
        }
        else
        {
            //Debug.LogAssertion("Stars count is incorrect");
        }
    }
}

