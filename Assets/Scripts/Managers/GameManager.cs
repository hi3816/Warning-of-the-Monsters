using System.Collections;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        
        // 매니저 중 DontDestroyOnLoad 인 인스턴스만 주석 활성화하여 사용
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    
    public void GameClear()
    {
        StageManager.Instance.CaculateStars();  // 플레이 정보로 별 계산
        StageManager.Instance.SavePlayData();  // 스테이지 플레이 정보 저장
        StartCoroutine(EndGameProcess<WinPopup>());
        //EndGame<WinPopup>();
    }
    
    public void GameOver()
    {
        StartCoroutine(EndGameProcess<LosePopup>());
        //EndGame<LosePopup>();
    }

    private IEnumerator EndGameProcess<T>() where T : UIBase
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        UIManager.Instance.Show<T>("UI/UIPopup/");
    }
    
    // private void EndGame<T>() where T : UIBase
    // {
    //     Time.timeScale = 0;
    //     UIManager.Instance.Show<T>("UI/UIPopup/");
    // }
}
