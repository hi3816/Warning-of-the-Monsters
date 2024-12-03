using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonBase<DataManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public List<int> GetMonsterIdListData()
    {
        List<TestTable.Data> monsterList = TestTable.Data.GetList();

        List<int> ids = new List<int>();
        foreach (var monster in monsterList)
        {
            ids.Add(monster.Id);
        }
        
        return ids;
    }
}
