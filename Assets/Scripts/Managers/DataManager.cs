using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : SingletonBase<DataManager>
{
    private StageSO[] _stageSOs;
    private TestSO[] _testSOs;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        //data캐싱
        _stageSOs = SetStageSOs();
        _testSOs = SetTestSOs();
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
            stageSOs[i].interWaveDelay = stageDataList[i].interWaveDelay;
        }

        return stageSOs;
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
}
