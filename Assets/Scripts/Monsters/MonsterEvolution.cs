using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MonsterEvolution : MonoBehaviour
{
    private Monster _selectMonster;

    [Header("Pool")]
    [SerializeField] private PoolManager.PoolConfig[] _poolConfigs;
    [SerializeField] private List<Monster> _minionMonsters;
    [SerializeField] private List<Monster> _evolutionMonsterList;

    // 진화 스프라이트, 필요재화 담을 Dictionary
    private Dictionary<int, Sprite[]> _evolutionSpriteDict = new Dictionary<int, Sprite[]>();
    private Dictionary<int, int[]> _evolutionRequiredCoinsDict = new Dictionary<int, int[]>();

    private MonsterEvolutionUI _monsterEvolutionUI;

    private void Awake()
    {
        _monsterEvolutionUI = GetComponent<MonsterEvolutionUI>();
        SetSprite(_poolConfigs);
        SetSprite(_minionMonsters);
        SetEvolutionData(_poolConfigs);
        SetEvolutionData(_minionMonsters);
        PoolManager.Instance.AddPools(_poolConfigs);
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

                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!_evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        _evolutionSpriteDict.Add(monsterID, spriteArr);
                    }
                }
            }
        }
    }

    private void SetSprite(List<Monster> list)
    {
        var selectedMonsterData = DataManager.Instance.selectedMonsterData;

        for (int i = 0; i < list.Count; i += 2)
        {
            Monster monster = list[i];
            int monsterID = GetIdByType(monster);

            foreach (int key in selectedMonsterData.Keys)
            {
                if (monsterID == selectedMonsterData[key].Item1)
                {
                    SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterEvolutionSprite");

                    Sprite sprite1 = _sprites.GetSprite(list[i].name);
                    Sprite sprite2 = _sprites.GetSprite(list[i + 1].name);
                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!_evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        _evolutionSpriteDict.Add(monsterID, spriteArr);
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
            _evolutionMonsterList.Add(monster1);
            _evolutionMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            _evolutionRequiredCoinsDict.Add(monsterId1, requiredCoins);
        }
    }

    private void SetEvolutionData(List<Monster> list)
    {
        for (int i = 0; i < list.Count; i += 2)
        {
            Monster monster1 = list[i];
            Monster monster2 = list[i + 1];
            int monsterId1 = GetIdByType(monster1);
            int monsterId2 = GetIdByType(monster2);
            monster1.Evolution(DataManager.Instance.GetEvolutionData(monsterId1, monster1.data.maxLevel, EvolutionType.Atype));
            monster2.Evolution(DataManager.Instance.GetEvolutionData(monsterId2, monster2.data.maxLevel, EvolutionType.Btype));
            _evolutionMonsterList.Add(monster1);
            _evolutionMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            _evolutionRequiredCoinsDict.Add(monsterId1, requiredCoins);
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
        _selectMonster = monster;
        if (_selectMonster == null) return;
        if (!_monsterEvolutionUI.SelectEvolutionMonster(evolutionType)) return; 

        var evolution = DataManager.Instance.GetEvolutionData(_selectMonster.data.id, _selectMonster.data.currentLevel + 1, evolutionType);
        if (evolution != null && StageManager.Instance.CurrGold >= evolution.requiredCoins)
        {
            StageManager.Instance.ChangeGold(-evolution.requiredCoins);

            if (evolution.monsterType == MonsterType.Summoner)
            {
                _selectMonster.Evolution(DataManager.Instance.GetEvolutionData(_selectMonster.data.id, _selectMonster.data.maxLevel, evolutionType));
                summonerMonster summoner = _selectMonster as summonerMonster;
                summoner.InitializeSummonableMinions();
            }
            else
            {
                string evolutionMonsterName = GetMonsterEvolutionName(evolutionType);
                Vector3 pos = _selectMonster.gameObject.transform.position;
                PoolManager.Instance.ReturnToPool(_selectMonster.data.poolTag, _selectMonster.gameObject);
                GameObject evolutionMonster = PoolManager.Instance.SpawnFromPool(evolutionMonsterName, pos, Quaternion.identity);
                Monster _monster = evolutionMonster.GetComponent<Monster>();
                _monster.Evolution(DataManager.Instance.GetEvolutionData(_monster.data.id, _monster.data.maxLevel, evolutionType));
            }
            _monsterEvolutionUI.Hide();
        }
    }

    // 진화 프리팹 tag 가져오기
    private string GetMonsterEvolutionName(EvolutionType evolutionType)
    {
        for (int i = 0; i < _evolutionMonsterList.Count; i += 2)
        {
            Monster monster = _evolutionMonsterList[i];
            int monsterID = monster.data.id;

            if (monsterID == _selectMonster.data.id)
            {
                if (evolutionType == EvolutionType.Atype)
                {
                    return _evolutionMonsterList[i].gameObject.name;
                }
                else if (evolutionType == EvolutionType.Btype)
                {
                    return _evolutionMonsterList[i + 1].gameObject.name;
                }
            }
        }
        return null;
    }

    // 미리 캐싱해둔 데이터 가져오기
    private Monster GetMonsterEvolutionData(EvolutionType evolutionType)
    {
        for (int i = 0; i < _evolutionMonsterList.Count; i += 2)
        {
            Monster monster = _evolutionMonsterList[i];
            int monsterID = monster.data.id;

            if (monsterID == _selectMonster.data.id)
            {
                if (evolutionType == EvolutionType.Atype)
                {
                    return _evolutionMonsterList[i];
                }
                else if (evolutionType == EvolutionType.Btype)
                {
                    return _evolutionMonsterList[i + 1];
                }
            }
        }
        return null;
    }

    public Sprite[] GetvolutionSpriteDict(int id)
    {
        if (!_evolutionSpriteDict.ContainsKey(id)) return null;
        return _evolutionSpriteDict[id];
    }

    public int[] GetEvolutionRequiredCoinsDict(int id)
    {
        if (!_evolutionRequiredCoinsDict.ContainsKey(id)) return null;
        return _evolutionRequiredCoinsDict[id];
    }
}
