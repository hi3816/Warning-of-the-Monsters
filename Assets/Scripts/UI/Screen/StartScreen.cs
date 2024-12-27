using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StartScreen : UIBase
{
    [SerializeField] private Button _startButton;

    [Header("Title")]
    [SerializeField] private CanvasGroup _titleCanvasGroup;
    [SerializeField] private Transform _titleTransform;

    [Header("Button")]
    [SerializeField] private CanvasGroup _buttonCanvasGroup;
    [SerializeField] private Transform _buttonTransform;
    // Start is called before the first frame update
    void Start()
    {
        _startButton.onClick.AddListener(OnButtonClicked);
        ShowTitle();
        ShowButton();
    }

    public void OnButtonClicked()
    {
        MySceneManager.Instance.ChangeScene("LobbyScene");
        //SceneManager.LoadScene("LobbyScene");
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

        _titleTransform.DOLocalMove(new Vector3(0, 10, 0), 0.5f)
            .SetRelative();
    }

    private void ShowButton()
    {
        _buttonCanvasGroup.DOFade(1, 0.3f);

        _buttonTransform.DOLocalMove(new Vector3(0, 10, 0), 0.5f)
            .SetRelative();
    }
}
