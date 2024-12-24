using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class StagePlayInfo
{
    public int stageIdx;
    public bool isCleared;
    public int starsCount;
}

[System.Serializable]
public class GamePlayInfo
{
    public List<StagePlayInfo> playInfos = new List<StagePlayInfo>();
}

public class SaveManager : SingletonBase<SaveManager>
{
    private const string _saveFileName = "PlayInfoData.json";

    private GamePlayInfo _gamePlayInfo = new GamePlayInfo();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    
    private void Start()
    {
        LoadGameData();
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, _saveFileName);
    }

    private void SaveGameData()
    {
        string json = JsonUtility.ToJson(_gamePlayInfo);
        File.WriteAllText(GetSavePath(), json);
    }

    private void LoadGameData()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _gamePlayInfo = JsonUtility.FromJson<GamePlayInfo>(json);
        }
        else
        {
            InitializeDefaultData();
        }
    }

    private void InitializeDefaultData()
    {
        _gamePlayInfo.playInfos.Clear();
        for (int i = 0; i < 10; i++)
        {
            _gamePlayInfo.playInfos.Add(new StagePlayInfo
            {
                stageIdx = i,
                isCleared = false,
                starsCount = 0
            });
        }
        SaveGameData();
    }

    public void UpdatePlayInfo(int stageIdx, int starsCount, bool isCleared)
    {
        StagePlayInfo info = _gamePlayInfo.playInfos.Find(x => x.stageIdx == stageIdx);
        if (info != null)
        {
            info.isCleared |= isCleared; // 이미 클리어한 경우에는 클리어 상태 유지
            if (starsCount > info.starsCount)    // 별 개수 더 많을 때만 수량 갱신
            {
                info.starsCount = starsCount;
            }
            SaveGameData();
        }
    }

    public void GetStagePlayInfo(int stageIdx, out int starsCount, out bool isCleared)
    {
        StagePlayInfo info = _gamePlayInfo.playInfos.Find(x => x.stageIdx == stageIdx);
        if (info != null)
        {
            starsCount = info.starsCount;
            isCleared = info.isCleared;
        }
        else
        {
            Debug.LogAssertion("Stage Play Info Not Found");
            starsCount = 0;
            isCleared = false;
        }
    }
    
    public void GetStagePlayInfo(int stageIdx, out bool isCleared)
    {
        StagePlayInfo info = _gamePlayInfo.playInfos.Find(x => x.stageIdx == stageIdx);
        if (info != null)
        {
            isCleared = info.isCleared;
        }
        else
        {
            Debug.LogAssertion("Stage Play Info Not Found");
            isCleared = false;
        }
    }
}
