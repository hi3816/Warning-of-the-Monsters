using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private List<Human> activeHumans = new List<Human>();
    private List<Monster> activeMonsters = new List<Monster>();
    
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

    public void AddActiveList<T>(T obj)
    {
        if (obj is Human)
        {
            activeHumans.Add(obj as Human);
            return;
        }

        if (obj is Monster)
        {
            activeMonsters.Add(obj as Monster);
        }
    }
    
    public void RemoveActiveList<T>(T obj)
    {
        if (obj is Human)
        {
            activeHumans.Remove(obj as Human);
            return;
        }

        if (obj is Monster)
        {
            activeMonsters.Remove(obj as Monster);
        }
    }

    private void OnDestroy()
    {
        ReturnObjects();
    }
}