using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : SingletonBase<DataManager>
{
    private StageSO[] _stageSOs;
    private TestSO[] _testSOs;
    private Dictionary<SfxType, float> _individualSfxVolumeDict;
    private SkillSO[] _skillSOs;

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
            stageSOs[i].name = $"StageSO{i + 1}";
            stageSOs[i].wave = stageDataList[i].wave;
            stageSOs[i].health = stageDataList[i].health;
            stageSOs[i].gold = stageDataList[i].gold;
            stageSOs[i].waveStartDelay = stageDataList[i].waveStartDelay;
            stageSOs[i].interWaveDelay = stageDataList[i].interWaveDelay;
        }

        return stageSOs;
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
            skillSOs[i].power = skillData[i].power;
            skillSOs[i].range = skillData[i].range;
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
