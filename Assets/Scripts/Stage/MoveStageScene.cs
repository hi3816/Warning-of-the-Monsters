using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveStageScene : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(LoadStageScene);
    }

    private void LoadStageScene()
    {
        SceneManager.LoadScene("StageScene");
    }
}
