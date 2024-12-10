using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : SingletonBase<HumanManager>
{
    [SerializeField] private List<bool> activeHumans = new List<bool>();
    public bool isLastWave;

    public Action OnGameClear;

    private void OnEnable()
    {
        isLastWave = false;
    }

    public void AddHumanList()
    {
        if (!isLastWave)
           isLastWave = true;
        activeHumans.Add(true);
    }

    public void RemoveHumanList()
    {
        activeHumans.Remove(true);
        if (isLastWave && activeHumans.Count == 0)
            OnGameClear?.Invoke();
    }
}