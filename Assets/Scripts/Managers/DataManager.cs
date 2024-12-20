using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonBase<DataManager>
{
    private StageSO[] _stageSOs;
    private TestSO[] _testSOs;
    private Dictionary<SfxType, float> _individualSfxVolumeDict;
    private SkillSO[] _skillSOs;
    private MonsterSO[] _monsterSOs;

    public Dictionary<int, (int,string)> SelectedMonsterData;

    public int selectedStageIdx;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        //data캐싱
        _stageSOs = SetStageSOs();
        _testSOs = SetTestSOs();
        _individualSfxVolumeDict = SetIndividualSfxVolumeDict();
        _skillSOs = SetSkillSOs();
        _monsterSOs = SetMonsterSOs();
    }

    private TestSO[] SetTestSOs()
    {
        List<TestTable.Data> testDataList = TestTable.Data.GetList();

        TestSO[] testSOs = new TestSO[testDataList.Count];
        for (int i = 0; i < testSOs.Length; i++)
        {
            testSOs[i] = ScriptableObject.CreateInstance<TestSO>();
            testSOs[i].id = testDataList[i].Id;
            testSOs[i].testName = testDataList[i].name;
            testSOs[i].testSpriteName = testDataList[i].spriteName;
        }
        return testSOs;
    }

    private StageSO[] SetStageSOs()
    {
        List<Stage_Data.Stage_Data> stageDataList = Stage_Data.Stage_Data.GetList();

        StageSO[] stageSOs = new StageSO[stageDataList.Count];
        for (int i = 0; i < stageSOs.Length; i++)
        {
            stageSOs[i] = ScriptableObject.CreateInstance<StageSO>();
            stageSOs[i].name = $"Stage{i + 1}";
            stageSOs[i].wave = stageDataList[i].wave;
            stageSOs[i].health = stageDataList[i].health;
            stageSOs[i].gold = stageDataList[i].gold;
            stageSOs[i].waveStartDelay = stageDataList[i].waveStartDelay;
            stageSOs[i].interWaveDelay = stageDataList[i].interWaveDelay;
        }

        return stageSOs;
    }

    private MonsterSO[] SetMonsterSOs()
    {
        List<Monster_Data.Monster_Data> monsterDataList = Monster_Data.Monster_Data.GetList();

        MonsterSO[] monsterSOs = new MonsterSO[monsterDataList.Count];
        for (int i = 0; i < monsterSOs.Length; i++)
        {
            monsterSOs[i] = ScriptableObject.CreateInstance<MonsterSO>(); // 인스턴스 생성
            monsterSOs[i].id = monsterDataList[i].id;
            monsterSOs[i].poolTag = monsterDataList[i].name;
            monsterSOs[i].fatigue = monsterDataList[i].fatigue;
            monsterSOs[i].minFearInflicted = monsterDataList[i].minFearInflicted;
            monsterSOs[i].maxFearInflicted = monsterDataList[i].maxFearInflicted;
            monsterSOs[i].cooldown = monsterDataList[i].cooldown;
            monsterSOs[i].humanScaringRange = monsterDataList[i].humanScaringRange;
            monsterSOs[i].requiredCoins = monsterDataList[i].requiredCoins;
            monsterSOs[i].monsterType = monsterDataList[i].MonsterType;
        }
        return monsterSOs;
    }

    private Dictionary<SfxType, float> SetIndividualSfxVolumeDict()
    {
        List<SfxVolume_Data.SfxVolume_Data> sfxVolumeDataList = SfxVolume_Data.SfxVolume_Data.GetList();

        Dictionary<SfxType, float> individualSfxVolumeDict = new Dictionary<SfxType, float> ();
        for (int i = 0; i < sfxVolumeDataList.Count; i++)
        {
            individualSfxVolumeDict.Add(sfxVolumeDataList[i].SfxType, sfxVolumeDataList[i].volume);
        }

        return individualSfxVolumeDict;
    }

    private SkillSO[] SetSkillSOs()
    {
        List<Skill_Data.Skill_Data> skillData = Skill_Data.Skill_Data.GetList();

        SkillSO[] skillSOs = new SkillSO[skillData.Count];
        for (int i = 0; i < skillSOs.Length; i++)
        {
            skillSOs[i] = ScriptableObject.CreateInstance<SkillSO>();
            skillSOs[i].id = skillData[i].id;
            skillSOs[i].skillName = skillData[i].SkillName;
            skillSOs[i].skillType = skillData[i].SkillType;
            skillSOs[i].power = skillData[i].power;
            skillSOs[i].range = skillData[i].range;
            skillSOs[i].percentage = skillData[i].percentage;
            skillSOs[i].cooldown = skillData[i].cooldown;
            skillSOs[i].duration = skillData[i].duration;
        }

        return skillSOs;
    }

    public TestSO[] GetTestSprite()
    {
        if (_testSOs == null)
        {
            _testSOs = SetTestSOs();
        }
        return _testSOs;
    }

    public StageSO GetStageByIndex(int idx)
    {
        if (_stageSOs == null)
        {
            _stageSOs = SetStageSOs();
        }
        return _stageSOs[idx];
    }

    public MonsterSO[] GetMonsterSOs()
    {
        if (_monsterSOs == null)
        {
            _monsterSOs = SetMonsterSOs();
        }
        return _monsterSOs;
    }

    public Dictionary<int, (int, string)> GetSelectedMonstersData()
    {
        if (SelectedMonsterData == null)
        {
            Debug.Log("선택된 몬스터 정보를 가져오지 못했습니다.");
        }
        return SelectedMonsterData;
    }

    public Dictionary<SfxType, float> GetIndvidualSfxVolumeDict()
    {
        if(_individualSfxVolumeDict == null)
        {
            _individualSfxVolumeDict = SetIndividualSfxVolumeDict();
        }
        return _individualSfxVolumeDict;
    }

    public SkillSO GetSkillByIndex(int idx)
    {
        if (_skillSOs == null)
        {
            _skillSOs = SetSkillSOs();
        }
        return _skillSOs[idx];
    }

    //StageData Cache Clearing
    public void ClearStageData()
    {
        if (_stageSOs != null)
        {
            foreach (var so in _stageSOs)
            {
                ScriptableObject.Destroy(so);
            }
            _stageSOs = null;
        }
    }

    //TestData Cache Clearing
    public void ClearTestData()
    {
        if (_testSOs != null)
        {
            foreach (var so in _testSOs)
            {
                ScriptableObject.Destroy(so);
            }
            _testSOs = null;
        }
    }

    public void ClearIndividualVolumeDict()
    {
        if(_individualSfxVolumeDict != null)
        {
            _individualSfxVolumeDict.Clear();
            _individualSfxVolumeDict = null;
        }
    }

    public void ClearSkillData()
    {
        if (_skillSOs != null)
        {
            foreach (var so in _skillSOs)
            {
                ScriptableObject.Destroy(so);
            }
            _skillSOs = null;
        }
    }
}
