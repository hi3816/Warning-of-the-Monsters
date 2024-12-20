using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadStageScene : MonoBehaviour
{
    public void ClickStageSceneBtn()
    {
        SceneManager.LoadScene("StageScene");
    }
}
