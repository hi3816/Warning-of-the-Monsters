using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpriteData
{
    public int id;
    public string spriteName;
}

public class DataManager : SingletonBase<DataManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public List<MonsterSpriteData> GetMonsterSpriteData()
    {
        var selectedData = TestTable.Data.GetList()
        .Select(monster => new MonsterSpriteData
        {
            id = monster.Id,
            spriteName = monster.spriteName
        })
        .ToList();

        return selectedData;
    }
}
