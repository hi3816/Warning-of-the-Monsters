using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager6 : SingletonBase<DataManager6>
{
    public Dictionary<int, (int, string)> selectedMonsterData;

    public int selectedStageIdx;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public List<DataTable.Monster_Data> GetMonsterSOs()
    {
        List<DataTable.Monster_Data> data = DataTable.Monster_Data.GetList();

        return data;
    }

    public Dictionary<int, (int, string)> GetSelectedMonstersData()
    {
        if (selectedMonsterData == null)
        {
            Debug.Log("선택된 몬스터 정보를 가져오지 못했습니다.");
        }
        return selectedMonsterData;
    }

    public DataTable.Stage_Data GetStageByIndex(int idx)
    {
        List<DataTable.Stage_Data>data = DataTable.Stage_Data.GetList();
        return data[idx];
    }
}
