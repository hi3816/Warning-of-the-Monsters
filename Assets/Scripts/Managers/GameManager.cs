using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
    public bool isPlaying;
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
        HumanSpawner.Instance.StopSpawningHumans(); // 인간 스폰 코루틴 중지
        for (int i = _activeHumans.Count - 1; i >= 0; i--)
        {
            _activeHumans[i].controller.Agent.speed = 0;    // 인간 이동 중지시키기
        }
    }

    private IEnumerator EndGameProcess<T>() where T : UIBase
    {
        //Time.timeScale = 0;
        isPlaying = false;
        yield return new WaitForSecondsRealtime(0.5f);
        UIManager.Instance.ShowPopup<T>();
    }

    public void ReturnObjects()
    {
        HumanSpawner.Instance.StopSpawningHumans();  // 인간 스폰 코루틴 중지
        
        // 활성화된 인간이나 몬스터 오브젝트 풀에 반환
        for (int i = _activeHumans.Count - 1; i >= 0; i--)
        {
            if (_activeHumans[i] == null) continue;
            string objectName = _activeHumans[i].name;
            PoolManager.Instance.ReturnToPool(objectName, _activeHumans[i]);
            _activeHumans.RemoveAt(i);
        }
        for (int i = _activeMinons.Count - 1; i >= 0; i--)
        {
            if (_activeMinons[i] == null) continue;
            string objectName = _activeMinons[i].name;
            PoolManager.Instance.ReturnToPool(objectName, _activeMinons[i]);
            _activeMinons.RemoveAt(i);
        }
        for (int i = _activeMonsters.Count - 1; i >= 0; i--)
        {
            if (_activeMonsters[i] == null) continue;
            string objectName = _activeMonsters[i].name;
            PoolManager.Instance.ReturnToPool(objectName, _activeMonsters[i]);
            _activeMonsters.RemoveAt(i);
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
}