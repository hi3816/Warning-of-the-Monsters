using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreen : UIBase
{
    public Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        MySceneManager.Instance.ChangeScene("LobbyScene");
        //SceneManager.LoadScene("LobbyScene");
    }
}
