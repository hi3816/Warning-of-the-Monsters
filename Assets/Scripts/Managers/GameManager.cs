using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        
        // 매니저 중 DontDestroyOnLoad 인 인스턴스만 주석 활성화하여 사용
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        HumanManager.Instance.OnGameClear += GameClear;
        StageManager.Instance.OnGameOver += GameOver;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void GameClear()
    {
        // Time.timeScale = 0;
        UIManager.Instance.Show<WinPopup>("UI/UIPopup/");
    }
    
    private void GameOver()
    {
        // Time.timeScale = 0;
        UIManager.Instance.Show<LosePopup>("UI/UIPopup/");
    }
}
