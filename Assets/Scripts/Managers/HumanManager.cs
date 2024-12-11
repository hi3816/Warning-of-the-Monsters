using System;
using System.Collections.Generic;
using UnityEngine;
using Wave_Data;

public enum HumanType
{
    NormalHuman,
    StrongHuman,
}
public class HumanManager : SingletonBase<HumanManager>
{
    //[SerializeField] private List<bool> activeHumans = new List<bool>();
    public List<int> countPerWave = new List<int>();
    public bool isLastWave;
    
    private int _currentWave;
    private int _totalHumansInWave;
    private Wave_Data.Stage1 _waveData;
    
    public Action<int> OnWaveCleared;
    public Action OnGameClear;

    private void OnEnable()
    {
        for (int i = 0; i < StageManager.Instance.totalWave; i++)
        {
            for (int j = i; j < _waveData.count.Count; j++)
            {
                //countPerWave.Add(_waveData.count[j]);
            }
        }
    }

    // public void AddHumanList()
    // {
    //     if (!isLastWave)
    //        isLastWave = true;
    //     activeHumans.Add(true);
    // }
    //
    // public void RemoveHumanList()
    // {
    //     activeHumans.Remove(true);
    //     if (isLastWave && activeHumans.Count == 0)
    //         OnGameClear?.Invoke();
    // }
    public void RemoveHumanList(int waveIdx)
    {
        countPerWave[waveIdx]--;

        if (countPerWave[waveIdx] == 0)
        {
            OnWaveCleared?.Invoke(waveIdx);
            
            if (waveIdx == StageManager.Instance.totalWave)
            {
                OnGameClear?.Invoke();
                return;
            }
        }
    }
}