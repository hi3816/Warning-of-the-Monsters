using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public List<Human> activeHumans = new List<Human>();
    public List<Monster> activeMonsters = new List<Monster>();
    
    protected override void Awake()
    {
        base.Awake();
        
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
        // 활성화된 인간이나 몬스터 오브젝트 풀에 반환
        while (activeHumans.Count > 0)
        {
            string objectName = activeHumans[0].name;
            PoolManager.Instance.ReturnToPool(objectName, activeHumans[0]);
        }
        while (activeMonsters.Count > 0)
        {
            string objectName = activeMonsters[0].name;
            PoolManager.Instance.ReturnToPool(objectName, activeMonsters[0]);
        }
    }

    private void OnDestroy()
    {
        ReturnObjects();
    }
}
