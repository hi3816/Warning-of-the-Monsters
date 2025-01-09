using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MonsterEvolution : MonoBehaviour
{
    private Monster _selectMonster;

    [Header("Pool")]
    [SerializeField] private PoolManager.PoolConfig[] _poolConfigs;
    [SerializeField] private List<Monster> _summonerMonsters;
    [SerializeField] private List<MonsterData> _evolutionMonsterList;

    // 진화 스프라이트, 필요재화 담을 Dictionary
    private Dictionary<int, Sprite[]> _evolutionSpriteDict = new Dictionary<int, Sprite[]>();
    private Dictionary<int, int[]> _evolutionRequiredCoinsDict = new Dictionary<int, int[]>();

    private MonsterEvolutionUI _monsterEvolutionUI;

    private void Awake()
    {
        _monsterEvolutionUI = GetComponent<MonsterEvolutionUI>();
        SetSprite(_poolConfigs);
        SetSprite(_summonerMonsters);
        SetEvolutionData(_poolConfigs);
        SetEvolutionData(_summonerMonsters);
        PoolManager.Instance.AddPools<Monster>(_poolConfigs);
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

                    Sprite sprite1 = _sprites.GetSprite(pools[i].prefab.GetComponent<Monster>().data.poolTag);
                    Sprite sprite2 = _sprites.GetSprite(pools[i+1].prefab.GetComponent<Monster>().data.poolTag);

                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!_evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        _evolutionSpriteDict[monsterID] = spriteArr;
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
            int monsterID = monster.data.id;

            foreach (int key in selectedMonsterData.Keys)
            {
                if (monsterID == selectedMonsterData[key].Item1)
                {
                    SpriteAtlas _sprites = Resources.Load<SpriteAtlas>("UI/UISprites/MonsterEvolutionSprite");

                    string[] stringArr = DataManager.Instance.GetEvolutionMinionNameBySummonerId(monsterID);
                    Sprite sprite1 = _sprites.GetSprite(stringArr[0]);
                    Sprite sprite2 = _sprites.GetSprite(stringArr[1]);
                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!_evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        _evolutionSpriteDict[monsterID] = spriteArr;
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
            _evolutionMonsterList.Add(monster1.data.Clone());
            _evolutionMonsterList.Add(monster2.data.Clone());

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            _evolutionRequiredCoinsDict[monsterId1] = requiredCoins;
        }
    }

    private void SetEvolutionData(List<Monster> summonerList)
    {
        for (int i = 0; i < summonerList.Count; i += 2)
        {
            Monster monster1 = summonerList[i];
            Monster monster2 = summonerList[i + 1];
            int monsterId1 = monster1.data.id;
            int monsterId2 = monster2.data.id;

            monster1.Evolution(DataManager.Instance.GetEvolutionData(monsterId1, monster1.data.maxLevel, EvolutionType.Atype));
            _evolutionMonsterList.Add(monster1.data.Clone());

            monster2.Evolution(DataManager.Instance.GetEvolutionData(monsterId2, monster2.data.maxLevel, EvolutionType.Btype));
            _evolutionMonsterList.Add(monster2.data.Clone());

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            _evolutionRequiredCoinsDict[monsterId1] = requiredCoins;
        }
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

            if (_selectMonster.data.monsterType == MonsterType.Summoner)
            {
                _selectMonster.data = GetMonsterEvolutionData(evolutionType).Clone();
                summonerMonster summoner = _selectMonster as summonerMonster;
                summoner.InitializeSummonableMinions();
                summoner.SetFatigue(0);
            }
            else
            {
                Vector3 pos = _selectMonster.gameObject.transform.position;
                string evolutionMonsterName = GetMonsterEvolutionName(evolutionType);
                GameManager.Instance.RemoveActiveList(_selectMonster);
                PoolManager.Instance.ReturnToPool(_selectMonster.data.poolTag, _selectMonster);
                Monster evolutionMonster = PoolManager.Instance.SpawnFromPool<Monster>(evolutionMonsterName, pos, Quaternion.identity);
                evolutionMonster.data = GetMonsterEvolutionData(evolutionType).Clone();
                GameManager.Instance.AddActiveList(evolutionMonster);
            }
            _monsterEvolutionUI.Hide();
        }
    }

    // 진화 프리팹 tag 가져오기
    private string GetMonsterEvolutionName(EvolutionType evolutionType)
    {
        for (int i = 0; i < _evolutionMonsterList.Count; i += 2)
        {
            MonsterData monster = _evolutionMonsterList[i];
            int monsterID = monster.id;

            if (monsterID == _selectMonster.data.id)
            {
                if (evolutionType == EvolutionType.Atype)
                {
                    return _evolutionMonsterList[i].poolTag;
                }
                else if (evolutionType == EvolutionType.Btype)
                {
                    return _evolutionMonsterList[i + 1].poolTag;
                }
            }
        }
        return null;
    }

    // 미리 캐싱해둔 데이터 가져오기
    private MonsterData GetMonsterEvolutionData(EvolutionType evolutionType)
    {
        for (int i = 0; i < _evolutionMonsterList.Count; i += 2)
        {
            MonsterData monster = _evolutionMonsterList[i];
            int monsterID = monster.id;

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
