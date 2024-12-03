using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePopup : MonoBehaviour
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
    }
    private void LoadGameScene()
    {
        SceneManager.LoadScene("StageScene");
    }
    private void LoadLobby()
    {
        SceneManager.LoadScene("LobySceneTest");
    }
}
