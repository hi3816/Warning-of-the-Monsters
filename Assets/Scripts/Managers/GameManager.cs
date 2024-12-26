using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public List<GameObject> activeObjects = new List<GameObject>();
    
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
    }
    
    public void GameOver()
    {
        StartCoroutine(EndGameProcess<LosePopup>());
    }

    private IEnumerator EndGameProcess<T>() where T : UIBase
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        UIManager.Instance.ShowPopup<T>();
    }

    public void ReturnObjects()
    {
        while (activeObjects.Count > 0)
        {
            string objectName = activeObjects[0].name;
            PoolManager.Instance.ReturnToPool(objectName, activeObjects[0]);
        }
    }
}
