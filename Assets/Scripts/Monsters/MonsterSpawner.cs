using System;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private StageManager stageManager;
    private Monster _monster;
    private Transform[] _spawnPoints;
    
    private void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("MonsterSpawnPoint"); //NO FIND...
        _spawnPoints = spawnPointObjects.Select(go => go.transform).ToArray();
    }
    
    private void SpawnMonster(Vector3 spawnPosition, MonsterSO selectedMonsterData)
    {
        GameObject monster = PoolManager.Instance.SpawnFromPool(selectedMonsterData.poolTag, spawnPosition, Quaternion.identity);
        if (monster != null)
        {
            monster.transform.position = spawnPosition;
            monster.SetActive(true);
            
            var monsterComponent = monster.GetComponent<Monster>();
            if (monsterComponent != null)
            {
                monsterComponent.data = selectedMonsterData;
                SpriteRenderer renderer = monsterComponent.GetComponent<SpriteRenderer>();
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 255f);
                monsterComponent.SetState(MonsterState.Idle);
            }
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //when player touch
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;
            
            foreach (Transform spawnPoint in _spawnPoints)
            {
                MonsterSO selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
                if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
                {
                    if (MonsterManager.Instance.SelectedMonsterId != 0)
                    {
                        if (stageManager.currGold >= selectedMonsterData.requiredCoins)
                        {
                            stageManager.currGold -= selectedMonsterData.requiredCoins;
                            Vector3 spawnPosition = spawnPoint.position;
                            SpawnMonster(spawnPosition, selectedMonsterData);

                        }
                        else
                        {
                            print("You do not have enough gold to spawn monster");
                        }
                    }
                }
            }
        }
    }
}