using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : SingletonBase<LoadingManager>
{
    public CanvasGroup fadeImg;
    float fadeDuration = 0.5f; //암전되는 시간.

    [SerializeField] private GameObject _loading;
    [SerializeField] private TextMeshProUGUI _loadingTxt;
    [SerializeField] private Image _loadingbar;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; // 이벤트에 추가
    }

    public void ChangeScene(string sceneName)
    {
        _loadingTxt.text = "0%";
        fadeImg.DOFade(1, fadeDuration)
            .OnStart(() =>
            {
                fadeImg.blocksRaycasts = true; //아래 레이캐스트 막기
            })
            .OnComplete(() =>
            {
                StartCoroutine(LoadScene(sceneName));
            });
    }

    IEnumerator LoadScene(string sceneName)
    {
        //loading.SetActive(true); //로딩 화면을 띄움

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; //퍼센트 딜레이용

        float past_time = 0;
        float percentage = 0;

        while (!(async.isDone))
        {
            yield return null;

            past_time += Time.deltaTime;

            if (percentage >= 90)
            {
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if (percentage == 100)
                {
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if (percentage >= 90) past_time = 0;
            }
            _loadingTxt.text = percentage.ToString("0") + "%"; //로딩 퍼센트 표기
            _loadingbar.fillAmount = percentage / 100;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트에서 제거*
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeImg.DOFade(0, fadeDuration)
        .OnStart(() => {
            //loading.SetActive(false);
        })
        .OnComplete(() => {
            fadeImg.blocksRaycasts = false;
            _loadingbar.fillAmount = 0;
        });
    }
}
