using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class StagePlayInfo
{
    public int StageIdx;
    public bool IsCleared;
    public int StarsCount;
}

[System.Serializable]
public class GamePlayInfo
{
    public List<StagePlayInfo> PlayInfos = new List<StagePlayInfo>();
}

public class SaveManager : SingletonBase<SaveManager>
{
    private const string SaveFileName = "PlayInfoData.json";

    private GamePlayInfo _gamePlayInfo = new GamePlayInfo();

    private void Awake()
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
        return Path.Combine(Application.persistentDataPath, SaveFileName);
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
        _gamePlayInfo.PlayInfos.Clear();
        for (int i = 0; i < 10; i++)
        {
            _gamePlayInfo.PlayInfos.Add(new StagePlayInfo
            {
                StageIdx = i,
                IsCleared = false,
                StarsCount = 0
            });
        }
        SaveGameData();
    }

    public void UpdatePlayInfo(int stageIdx, int starsCount, bool isCleared)
    {
        StagePlayInfo info = _gamePlayInfo.PlayInfos.Find(x => x.StageIdx == stageIdx);
        if (info != null)
        {
            info.IsCleared |= isCleared; // 이미 클리어한 경우에는 클리어 상태 유지
            if (starsCount > info.StarsCount)    // 별 개수 더 많을 때만 수량 갱신
            {
                info.StarsCount = starsCount;
            }
            SaveGameData();
        }
    }

    public void GetStagePlayInfo(int stageIdx, out int starsCount, out bool isCleared)
    {
        StagePlayInfo info = _gamePlayInfo.PlayInfos.Find(x => x.StageIdx == stageIdx);
        if (info != null)
        {
            starsCount = info.StarsCount;
            isCleared = info.IsCleared;
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
        StagePlayInfo info = _gamePlayInfo.PlayInfos.Find(x => x.StageIdx == stageIdx);
        if (info != null)
        {
            isCleared = info.IsCleared;
        }
        else
        {
            Debug.LogAssertion("Stage Play Info Not Found");
            isCleared = false;
        }
    }
}
