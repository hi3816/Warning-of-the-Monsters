using System.Collections.Generic;
using UnityEngine;

public class DataManager3 : SingletonBase<DataManager3>
{
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    public DataTable.Human_Data GetHumanByIndex(int idx)
    {
        DataTable.Human_Data.GetList();
        return DataTable.Human_Data.Human_DataList[idx];
    }
}
