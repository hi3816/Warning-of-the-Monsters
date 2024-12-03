using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonBase<UIManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private List<int> GetMonsterIdListData()
    {
        List<Monster_Data.Monster_Data> monsterList = Monster_Data.Monster_Data.GetList();

        List<int> ids = new List<int>();
        foreach (var monster in monsterList)
        {
            ids.Add(monster.id);
        }
        
        return ids;
    }
}
