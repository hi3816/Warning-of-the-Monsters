using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    protected StageManager StageManager;
    private Monster _monster;
    protected Transform[] SpawnPoints;
    
    private void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("MonsterSpawnPoint"); //NO FIND...
        SpawnPoints = spawnPointObjects.Select(go => go.transform).ToArray();
        StageManager = StageManager.Instance;
    }
    
    protected bool IsSpawnPointOccupied(Vector3 spawnPosition, float checkRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, checkRadius, LayerMask.GetMask("Monster"));
        foreach (var collider in colliders)
        {
            if (Vector3.Distance(spawnPosition, collider.transform.position) < checkRadius)
            {
                return true;
            }
        }
        return false;
    }
    
    protected void SpawnMonster(Vector3 spawnPosition, MonsterSO selectedMonsterData)
    {
        if (IsSpawnPointOccupied(spawnPosition, 0.5f))
        {
            print("Spawn point is already occupied by another monster.");
            return;
        }
        
        if (StageManager.CurrGold >= selectedMonsterData.requiredCoins)
        {
            StageManager.ChangeGold(-selectedMonsterData.requiredCoins);
            GameObject monster = PoolManager.Instance.SpawnFromPool(selectedMonsterData.poolTag, spawnPosition, Quaternion.identity);
            if (monster != null)
            {
                monster.transform.position = spawnPosition;
                monster.SetActive(true);

                var monsterComponent = monster.GetComponent<Monster>();
                if (monsterComponent != null)
                {
                    SetMonsterData(monsterComponent.data, selectedMonsterData);
                    monsterComponent.Reset();
                }
                
                // 나중에 big 몬스터인지 small 몬스터인지 판별하는 조건 추가
                SoundManager.Instance.PlaySFX(SfxType.SpawnSmallMonster);
            }
            else
            {
                print("You do not have enough gold to spawn monster");
            }
        }
    }

    private void SetMonsterData(MonsterData data, MonsterSO so)
    {
        data.id = so.id;
        data.minionId = so.minionId;
        data.monsterId = so.monsterId;
        data.currentLevel = so.upgradeLevel;
        data.poolTag = so.poolTag;
        data.fatigue = so.fatigue; 
        data.fearInflicted = so.fearInflicted; 
        data.cooldown = so.cooldown; 
        data.humanScaringRange = so.humanScaringRange; 
        data.speed = so.speed; 
        data.requiredCoins = so.requiredCoins; 
        data.maxLevel = so.maxLevel; 
    }

    // void Update()
    // {
    //     if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
    //     {
    //         return;
    //     }
    //
    //     if (Input.GetMouseButtonDown(0)) //when player touch
    //     {
    //         Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         touchPosition.z = 0f;
    //
    //         foreach (Transform spawnPoint in SpawnPoints)
    //         {
    //             MonsterSO selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
    //             if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
    //             {
    //                 if (MonsterManager.Instance.SelectedMonsterId != 0)
    //                 {
    //                     if (!IsSpawnPointOccupied(spawnPoint.position, 0.5f) && stageManager.CurrGold >= selectedMonsterData.requiredCoins)
    //                     {
    //                         Vector3 spawnPosition = spawnPoint.position;
    //                         SpawnMonster(spawnPosition, selectedMonsterData);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
}