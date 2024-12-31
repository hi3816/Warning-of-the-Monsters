using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MonsterEvolution : MonoBehaviour
{
    private Monster selectMonster;

    [System.Serializable]
    public class SummonerSpritePair
    {
        public Sprite sprite;
        public Monster monster;
    }

    [Header("Pool")]
    [SerializeField] private PoolManager.PoolConfig[] poolConfigs;
    [SerializeField] private List<SummonerSpritePair> summonerMonsters;
    [SerializeField] private List<Monster> evolutionMonsterList;

    // 진화 스프라이트, 필요재화 담을 Dictionary
    private Dictionary<int, Sprite[]> evolutionSpriteDict = new Dictionary<int, Sprite[]>();
    private Dictionary<int, int[]> evolutionRequiredCoinsDict = new Dictionary<int, int[]>();

    private MonsterEvolutionUI monsterEvolutionUI;
    private void Awake()
    {
        monsterEvolutionUI = GetComponent<MonsterEvolutionUI>();
        SetSprite(poolConfigs);
        SetSprite(summonerMonsters);
        SetEvolutionData(poolConfigs);
        SetEvolutionData(summonerMonsters);
        PoolManager.Instance.AddPools(poolConfigs);
    }

    private void SetSprite(PoolManager.PoolConfig[] pools)
    {
        // 로비씬에서 선택한 4개의 몬스터
        var selectedMonsterData = DataManager.Instance.selectedMonsterData;

        for (int i = 0; i < pools.Length; i += 2)
        {
            Monster monster = pools[i].prefab.GetComponent<Monster>();
            int monsterID = monster.data.id;

            foreach (int key in selectedMonsterData.Keys)
            {
                if (monsterID == selectedMonsterData[key].Item1)
                {
                    SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterEvolutionSprite");

                    Sprite sprite1 = _sprites.GetSprite(pools[i].prefab.name);
                    Sprite sprite2 = _sprites.GetSprite(pools[i+1].prefab.name);

                    //Sprite sprite1 = pools[i].prefab.GetComponent<SpriteRenderer>().sprite;
                    //Sprite sprite2 = pools[i + 1].prefab.GetComponent<SpriteRenderer>().sprite;
                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        evolutionSpriteDict.Add(monsterID, spriteArr);
                    }
                }
            }
        }
    }

    private void SetSprite(List<SummonerSpritePair> list)
    {
        var selectedMonsterData = DataManager.Instance.selectedMonsterData;

        for (int i = 0; i < list.Count; i += 2)
        {
            Monster monster = list[i].monster;
            int monsterID = GetIdByType(monster);

            foreach (int key in selectedMonsterData.Keys)
            {
                if (monsterID == selectedMonsterData[key].Item1)
                {
                    SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterEvolutionSprite");

                    Sprite sprite1 = _sprites.GetSprite(list[i].monster.name);
                    Sprite sprite2 = _sprites.GetSprite(list[i+1].monster.name);
                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        evolutionSpriteDict.Add(monsterID, spriteArr);
                    }
                }
            }
        }
    }

    private void SetEvolutionData(PoolManager.PoolConfig[] pools)
    {
        for (int i = 0; i < pools.Length; i += 2)
        {
            Monster monster1 = pools[i].prefab.GetComponent<Monster>();
            Monster monster2 = pools[i + 1].prefab.GetComponent<Monster>();
            int monsterId1 = monster1.data.id;
            int monsterId2 = monster2.data.id;
            monster1.Evolution(DataManager.Instance.GetEvolutionData(monsterId1, monster1.data.maxLevel, EvolutionType.Atype));
            monster2.Evolution(DataManager.Instance.GetEvolutionData(monsterId2, monster2.data.maxLevel, EvolutionType.Btype));
            evolutionMonsterList.Add(monster1);
            evolutionMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            evolutionRequiredCoinsDict.Add(monsterId1, requiredCoins);
        }
    }

    private void SetEvolutionData(List<SummonerSpritePair> list)
    {
        for (int i = 0; i < list.Count; i += 2)
        {
            Monster monster1 = list[i].monster;
            Monster monster2 = list[i + 1].monster;
            int monsterId1 = GetIdByType(monster1);
            int monsterId2 = GetIdByType(monster2);
            monster1.Evolution(DataManager.Instance.GetEvolutionData(monsterId1, monster1.data.maxLevel, EvolutionType.Atype));
            monster2.Evolution(DataManager.Instance.GetEvolutionData(monsterId2, monster2.data.maxLevel, EvolutionType.Btype));
            evolutionMonsterList.Add(monster1);
            evolutionMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            evolutionRequiredCoinsDict.Add(monsterId1, requiredCoins);
        }
    }

    private int GetIdByType(Monster monster)
    {
        if (monster.data.monsterType == MonsterType.Minion)
        {
            return DataManager.Instance.GetSummonerIdByEvolutionMinionId(monster.data.id);
        }
        return monster.data.id;
    }

    // 진화 (프리팹 변경)
    public void Evolution(Monster monster, EvolutionType evolutionType)
    {
        selectMonster = monster;
        if (selectMonster == null) return;
        if (!monsterEvolutionUI.SelectEvolutionMonster(evolutionType)) return; 

        var evolution = DataManager.Instance.GetEvolutionData(selectMonster.data.id, selectMonster.data.currentLevel + 1, evolutionType);
        if (evolution != null && StageManager.Instance.CurrGold >= evolution.requiredCoins)
        {
            StageManager.Instance.ChangeGold(-evolution.requiredCoins);

            if (evolution.monsterType == MonsterType.Summoner)
            {
                selectMonster.Evolution(DataManager.Instance.GetEvolutionData(selectMonster.data.id, selectMonster.data.maxLevel, evolutionType));
                summonerMonster summoner = selectMonster.GetComponent<summonerMonster>();
                summoner.InitializeSummonableMinions();
            }
            else
            {
                string evolutionMonsterName = GetMonsterEvolutionName(evolutionType);
                Vector3 pos = selectMonster.gameObject.transform.position;
                PoolManager.Instance.ReturnToPool(selectMonster.gameObject.name, selectMonster.gameObject);
                GameObject evolutionMonster = PoolManager.Instance.SpawnFromPool(evolutionMonsterName, pos, Quaternion.identity);
                Monster _monster = evolutionMonster.GetComponent<Monster>();
                _monster.Evolution(DataManager.Instance.GetEvolutionData(_monster.data.id, _monster.data.maxLevel, evolutionType));
            }

            monsterEvolutionUI.Hide();
        }
    }

    // 진화 프리팹 tag 가져오기
    private string GetMonsterEvolutionName(EvolutionType evolutionType)
    {
        for (int i = 0; i < evolutionMonsterList.Count; i += 2)
        {
            Monster monster = evolutionMonsterList[i];
            int monsterID = monster.data.id;

            if (monsterID == selectMonster.data.id)
            {
                if (evolutionType == EvolutionType.Atype)
                {
                    return evolutionMonsterList[i].gameObject.name;
                }
                else if (evolutionType == EvolutionType.Btype)
                {
                    return evolutionMonsterList[i + 1].gameObject.name;
                }
            }
        }
        return null;
    }

    // 미리 캐싱해둔 데이터 가져오기
    private Monster GetMonsterEvolutionData(EvolutionType evolutionType)
    {
        for (int i = 0; i < evolutionMonsterList.Count; i += 2)
        {
            Monster monster = evolutionMonsterList[i];
            int monsterID = monster.data.id;

            if (monsterID == selectMonster.data.id)
            {
                if (evolutionType == EvolutionType.Atype)
                {
                    return evolutionMonsterList[i];
                }
                else if (evolutionType == EvolutionType.Btype)
                {
                    return evolutionMonsterList[i + 1];
                }
            }
        }
        return null;
    }

    public Sprite[] GetvolutionSpriteDict(int id)
    {
        if (!evolutionSpriteDict.ContainsKey(id)) return null;
        return evolutionSpriteDict[id];
    }

    public int[] GetEvolutionRequiredCoinsDict(int id)
    {
        if (!evolutionRequiredCoinsDict.ContainsKey(id)) return null;
        return evolutionRequiredCoinsDict[id];
    }
}
