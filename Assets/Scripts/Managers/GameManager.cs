using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private List<Human> _activeHumans = new List<Human>();
    private List<Minion> _activeMinons = new List<Minion>();
    private List<Monster> _activeMonsters = new List<Monster>();
    
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
        ReturnObjects();
    }

    public void ReturnObjects()
    {
        HumanSpawner.Instance.StopSpawningHumans();
        
        // 활성화된 인간이나 몬스터 오브젝트 풀에 반환
        // for (int i = activeHumans.Count - 1; i >= 0; i--)
        // {
        //     string objectName = activeHumans[i].name;
        //     PoolManager.Instance.ReturnToPool(objectName, activeHumans[i]);
        //     activeHumans.RemoveAt(i);
        // }
        // for (int i = activeMinons.Count - 1; i >= 0; i--)
        // {
        //     string objectName = activeMinons[i].name;
        //     PoolManager.Instance.ReturnToPool(objectName, activeMinons[i]);
        //     activeMinons.RemoveAt(i);
        // }
        // for (int i = activeMonsters.Count - 1; i >= 0; i--)
        // {
        //     string objectName = activeMonsters[i].name;
        //     PoolManager.Instance.ReturnToPool(objectName, activeMonsters[i]);
        //     activeMonsters.RemoveAt(i);
        // }
        // TODO: index 에러 없이 for로 변경
        while (_activeHumans.Count > 0)
        {
            string objectName = _activeHumans[0].name;
            PoolManager.Instance.ReturnToPool(objectName, _activeHumans[0]);
            _activeHumans.RemoveAt(0);
        }
        while (_activeMinons.Count > 0)
        {
            string objectName = _activeMinons[0].name;
            PoolManager.Instance.ReturnToPool(objectName, _activeMinons[0]);
            _activeMinons.RemoveAt(0);
        }
        while (_activeMonsters.Count > 0)
        {
            string objectName = _activeMonsters[0].name;
            PoolManager.Instance.ReturnToPool(objectName, _activeMonsters[0]);
            _activeMonsters.RemoveAt(0);
        }
    }

    public void AddActiveList<T>(T obj)
    {
        if (obj is Human)
        {
            _activeHumans.Add(obj as Human);
            return;
        }
        
        if (obj is Minion)
        {
            _activeMinons.Add(obj as Minion);
            return;
        }

        if (obj is Monster)
        {
            _activeMonsters.Add(obj as Monster);
        }
    }
    
    public void RemoveActiveList<T>(T obj)
    {
        if (obj is Human)
        {
            _activeHumans.Remove(obj as Human);
            return;
        }
        
        if (obj is Minion)
        {
            _activeMinons.Remove(obj as Minion);
            return;
        }

        if (obj is Monster)
        {
            _activeMonsters.Remove(obj as Monster);
        }
    }

    private void OnDestroy()
    {
        ReturnObjects();
    }
}