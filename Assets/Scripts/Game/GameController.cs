using UnityEngine;

public class GameController : MonoBehaviour
{
    private void OnEnable()
    {
        HumanManager.Instance.OnGameClear -= GameClear;
        HumanManager.Instance.OnGameClear += GameClear;
        StageManager.Instance.OnGameOver -= GameOver;
        StageManager.Instance.OnGameOver += GameOver;
    }
    
    private void GameClear()
    {
        Time.timeScale = 0;
        UIManager.Instance.Show<WinPopup>("UI/UIPopup/");
    }
    
    private void GameOver()
    {
        Time.timeScale = 0;
        UIManager.Instance.Show<LosePopup>("UI/UIPopup/");
    }
}
