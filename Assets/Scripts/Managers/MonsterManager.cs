using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [Header("References")]
    [SerializeField] private MonsterSpawner monsterSpawner;
    
    public List<MonsterSO> selectedMonsters = new List<MonsterSO>(); // Four monsters selected in the stage scene
    private StageManager _stageManager;
    
    private int _selectedMonsterIndex = 0;
    public int SelectedMonsterIndex => _selectedMonsterIndex; // Current monster selected by the player
    
    private void Start()
    {
        LoadMonstersFromGoogleSheets();
    }
    
    private void LoadMonstersFromGoogleSheets()
    {
        // Fetch and parse the data from Google Sheets API here
        // After data is fetched, populate the selectedMonsters list
    }

    public void SelectMonster(int index)
    { 
        if (index >= 0 && index < selectedMonsters.Count)
        {
            _selectedMonsterIndex = index;
        }
    }

    public void SpawnMonsterAtPosition(Vector3 spawnPosition)
    {
        MonsterSO selectedMonster = selectedMonsters[_selectedMonsterIndex];
    
        // if (_stageManager.currGold >= selectedMonster.requiredCoins)
        // {
        //     _stageManager.currGold -= selectedMonster.requiredCoins;
        //
        //     if (monsterSpawner != null)
        //     {
        //         monsterSpawner.SpawnMonster(spawnPosition);
        //     }
        // }
        // else
        // {
        //     Debug.Log("Not enough coins to spawn this monster.");
        // }
    }
}