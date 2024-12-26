using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager5 : SingletonBase<DataManager5>
{
    private Dictionary<SfxType, float> _individualSfxVolumeDict;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void SetIndividualSfxVolumeDict()
    {
        List<DataTable.SfxVolume_Data> sfxVolumeDataList = DataTable.SfxVolume_Data.GetList();

        Dictionary<SfxType, float> individualSfxVolumeDict = new Dictionary<SfxType, float>();
        for (int i = 0; i < sfxVolumeDataList.Count; i++)
        {
            individualSfxVolumeDict.Add(sfxVolumeDataList[i].sfxType, sfxVolumeDataList[i].volume);
        }

        _individualSfxVolumeDict = individualSfxVolumeDict;
    }

    public DataTable.Stage_Data GetStageByIndex(int idx)
    {
        return DataTable.Stage_Data.GetList()[idx];
    }

    public DataTable.Skill_Data GetSkillByIndex(int idx)
    {
        return DataTable.Skill_Data.GetList()[idx];
    }

    public Dictionary<SfxType, float> GetIndvidualSfxVolumeDict()
    {
        if (_individualSfxVolumeDict == null)
        {
            SetIndividualSfxVolumeDict();
        }
        return _individualSfxVolumeDict;
    }

    // 진화 데이터 확인 (EvolutionType 상관 없을 때)
    public DataTable.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel)
    {
        foreach (var evolution in DataTable.Evolution_Data.GetList())
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolutionId); // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            if (baseEvolutionId == monsterId && level == upgradeLevel)
            {
                return evolution;
            }
        }
        return null;
    }

    // 진화 데이터 확인 (EvolutionType 있을 때) -> 진화 버튼 클릭 시 확인용
    public DataTable.Evolution_Data GetEvolutionData(int monsterId, int upgradeLevel, EvolutionType evolutionType)
    {
        foreach (var evolution in DataTable.Evolution_Data.GetList())
        {
            int baseEvolutionId = Mathf.FloorToInt(evolution.evolutionId); // base id (1, 2, etc.)
            int level = evolution.upgradeLevel;
            EvolutionType type = evolution.EvolutionType;
            if (baseEvolutionId == monsterId && level == upgradeLevel && type == evolutionType)
            {
                return evolution;
            }
        }
        return null;
    }
}
