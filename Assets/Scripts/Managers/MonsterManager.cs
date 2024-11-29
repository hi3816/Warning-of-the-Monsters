using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonBase<MonsterManager>
{
    [SerializeField] private Transform[] spawnPoints; // Spawn points in the game scene
    public List<MonsterSO> selectedMonsters = new List<MonsterSO>(); // Four monsters selected in the stage scene
    private StageManager _stageManager;
    private int _selectedMonsterIndex = 0; // Current monster selected by the player

    private void Start()
    {
        _stageManager = FindObjectOfType<StageManager>();

        if (selectedMonsters.Count == 0)
        {
            //플레이어가 몬스터를 선택하면 카운트 올라감
        }
    }

    public void SelectMonster(int index)
    {
        // Change the currently selected monster
        if (index >= 0 && index < selectedMonsters.Count)
        {
            _selectedMonsterIndex = index;
        }
    }

    // public void SpawnMonsterAtPosition(Vector3 spawnPosition)
    // {
    //     MonsterSO selectedMonster = selectedMonsters[_selectedMonsterIndex];
    //
    //     if (_stageManager.currGold >= selectedMonster.requiredCoins)
    //     {
    //         _stageManager.currGold -= selectedMonster.requiredCoins;
    //         MonsterSpawner spawner = FindObjectOfType<MonsterSpawner>(); //find말고 다른걸로 고치기
    //
    //         if (spawner != null)
    //         {
    //             spawner.SpawnMonster(spawnPosition);
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("Not enough coins to spawn this monster.");
    //     }
    // }
}