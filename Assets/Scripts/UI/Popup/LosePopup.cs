using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePopup : UIBase
{
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
        Time.timeScale = 1f;
        MySceneManager.Instance.ChangeScene("LobbyScene");
    }
}
