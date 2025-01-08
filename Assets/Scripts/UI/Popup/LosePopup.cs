using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePopup : UIBase
{
    [Header("LosePopup")]
    [SerializeField] RectTransform _losePopupPosition;

    [Header("Button")]
    public Button btnRetry;
    public Button btnExit;

    [Header("Display")]
    public TextMeshProUGUI displayResultInfo;

    private void Start()
    {
        btnRetry.onClick.AddListener(LoadGameScene);
        btnExit.onClick.AddListener(LoadLobby);

        //canvas order조절
        Canvas canavas = GetComponentInParent<Canvas>();
        canvas.sortingOrder = 4;
    }
    private void LoadGameScene()
    {
        MySceneManager.Instance.ChangeScene("MainScene");
    }
    private void LoadLobby()
    {
        MySceneManager.Instance.ChangeScene("LobbyScene");
    }

    private void OnEnable()
    {
        // 초기화
        _losePopupPosition.localScale = Vector3.zero;
        
        //시간 재개. 없으면 애니메이션이 실행안됨.
        Time.timeScale = 1f;
        // 애니메이션
        _losePopupPosition.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack);
    }
}
