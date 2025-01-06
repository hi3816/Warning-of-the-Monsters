using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StartScreen : UIBase
{
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _guidePanel;
    [SerializeField] private Button _guideButton;
    [SerializeField] private Button _guideExitButton;

    [Header("Title")]
    [SerializeField] private CanvasGroup _titleCanvasGroup;
    [SerializeField] private Transform _titleTransform;

    [Header("StartButton")]
    [SerializeField] private CanvasGroup _buttonCanvasGroup;
    [SerializeField] private Transform _buttonTransform;

    [Header("GuideButton")]
    [SerializeField] private CanvasGroup _guideButtonCanvasGroup;
    [SerializeField] private Transform _guideButtonTransform;

    // Start is called before the first frame update
    void Start()
    {
        _startButton.onClick.AddListener(OnButtonClicked);
        _guideButton.onClick.AddListener(OnGuideButtonCliked);
        _guideExitButton.onClick.AddListener(OnGuideExitButtonClicked);
        ShowTitle();
        ShowButton();
    }

    public void OnButtonClicked()
    {
        MySceneManager.Instance.ChangeScene("LobbyScene");
    }

    private void OnGuideButtonCliked()
    {
        _guidePanel.SetActive(true);
    }

    private void OnGuideExitButtonClicked()
    {
        _guidePanel.SetActive(false);
    }

    public void ShowTitle()
    {
        _titleCanvasGroup.DOFade(1, 0.3f)
            .OnStart(() =>
            {
                _titleCanvasGroup.blocksRaycasts = true; //아래 레이캐스트 막기
            })
            .OnComplete(() =>
            {
                _titleCanvasGroup.blocksRaycasts = false;
            });

        _titleTransform.DOLocalMove(Vector3.up * 10, 0.5f)
            .SetRelative();
    }

    private void ShowButton()
    {
        _buttonCanvasGroup.DOFade(1, 0.3f);

        _buttonTransform.DOLocalMove(Vector3.up * 10, 0.5f)
            .SetRelative();

        _guideButtonCanvasGroup.DOFade(1, 0.3f);

        _guideButtonTransform.DOLocalMove(Vector3.up * 10, 0.5f)
            .SetRelative();
    }
}
