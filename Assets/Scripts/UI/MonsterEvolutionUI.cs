using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterEvolutionUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject evolutionUI;
    [SerializeField] private Image[] typeImages;
    [SerializeField] private TextMeshProUGUI[] requiredCoins;
    [SerializeField] private Button typeButtonA;
    [SerializeField] private Button typeButtonB;

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
    private Dictionary<int, int[]> evolutionResuiredCoinsDict = new Dictionary<int, int[]>();

    private Monster selectMonster; // 현재 클릭한 몬스터

    private void Awake()
    {
        SetSprite(poolConfigs);
        SetSprite(summonerMonsters);
        typeButtonA.onClick.AddListener(() => MonsterEvolution(EvolutionType.Atype));
        typeButtonB.onClick.AddListener(() => MonsterEvolution(EvolutionType.Btype));
    }

    private void Start()
    {
        // DataManager.Instance.GetEvolutionData가 Awake에서는 동작 안함
        SetEvolutionData(poolConfigs);
        SetEvolutionData(summonerMonsters);
        PoolManager.Instance.AddPoolS(poolConfigs);
    }

    private void SetSprite(PoolManager.PoolConfig[] pools)
    {
        // 로비씬에서 선택한 4개의 몬스터
        var selectedMonsterData = DataManager.Instance.SelectedMonsterData;

        for (int i = 0; i < pools.Length; i+=2)
        {
            Monster monster = pools[i].prefab.GetComponent<Monster>();
            int monsterID = monster.data.id;

            foreach (int key in selectedMonsterData.Keys)
            {
                if(monsterID == selectedMonsterData[key].Item1)
                {
                    Sprite sprite1 = pools[i].prefab.GetComponent<SpriteRenderer>().sprite;
                    Sprite sprite2 = pools[i+1].prefab.GetComponent<SpriteRenderer>().sprite;
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
        var selectedMonsterData = DataManager.Instance.SelectedMonsterData;

        for (int i = 0; i < list.Count; i += 2)
        {
            Monster monster = list[i].monster;
            int monsterID = monster.data.id;

            foreach (int key in selectedMonsterData.Keys)
            {
                if (monsterID == selectedMonsterData[key].Item1)
                {
                    Sprite sprite1 = list[i].sprite;
                    Sprite sprite2 = list[i + 1].sprite;
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
            Monster monster2 = pools[i+1].prefab.GetComponent<Monster>();
            int monsterId1 = monster1.data.id;
            int monsterId2 = monster2.data.id;
            monster1.Evolution(MonsterDataManager.Instance.GetEvolutionData(monsterId1, monster1.data.maxLevel, EvolutionType.Atype));
            monster2.Evolution(MonsterDataManager.Instance.GetEvolutionData(monsterId2, monster2.data.maxLevel, EvolutionType.Btype));
            evolutionMonsterList.Add(monster1);
            evolutionMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            evolutionResuiredCoinsDict.Add(monsterId1, requiredCoins);
        }
    }

    private void SetEvolutionData(List<SummonerSpritePair> list)
    {
        for (int i = 0; i < list.Count; i += 2)
        {
            Monster monster1 = list[i].monster;
            Monster monster2 = list[i + 1].monster;
            int monsterId1 = monster1.data.id;
            int monsterId2 = monster2.data.id;
            monster1.Evolution(MonsterDataManager.Instance.GetEvolutionData(monsterId1, monster1.data.maxLevel, EvolutionType.Atype));
            monster2.Evolution(MonsterDataManager.Instance.GetEvolutionData(monsterId2, monster2.data.maxLevel, EvolutionType.Btype));
            evolutionMonsterList.Add(monster1);
            evolutionMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            evolutionResuiredCoinsDict.Add(monsterId1, requiredCoins);
        }
    }

    private int GetIdByType(Monster monster)
    {
        if (monster.data.monsterType == MonsterType.Minion) return monster.data.summonerId;
        return monster.data.id;
    }

    public void Show(Monster monster)
    {
        selectMonster = monster;
        Vector3 worldPosition = monster.transform.position;
        evolutionUI.transform.position = worldPosition + Vector3.up;
        evolutionUI.SetActive(true);
        ResetEvolutionPanel();
        SetEvolutionPanel();
    }

    public void Hide()
    {
        evolutionUI.SetActive(false);
    }

    private void ResetEvolutionPanel()
    {
        ResetEvolutionImageSprite();
        ResetRequiredCoinsText();
    }

    private void SetEvolutionPanel()
    {
        SetEnvolutionImageSprite();
        SetRequiredCoinsText();
    }

    // 진화 sprite 초기화
    private void ResetEvolutionImageSprite()
    {
        foreach (Image image in typeImages)
        {
            image.sprite = null;
        }
    }

    private void ResetRequiredCoinsText()
    {
        foreach (TextMeshProUGUI text in requiredCoins)
        {
            text.text = null;
        }
    }

    // 진화 이미지 설정
    private void SetEnvolutionImageSprite()
    {
        if (evolutionSpriteDict.ContainsKey(selectMonster.data.id))
        {
            Sprite[] sprites = evolutionSpriteDict[selectMonster.data.id];

            for (int i = 0; i < typeImages.Length; i++)
            {
                typeImages[i].sprite = sprites[i];
                typeImages[i].color = GetPurchaseStatusColor(selectMonster.data.id, i);
            }
        }
    }

    private void SetRequiredCoinsText()
    {
        if (evolutionResuiredCoinsDict.ContainsKey(selectMonster.data.id))
        {
            int[] coins = evolutionResuiredCoinsDict[selectMonster.data.id];

            for (int i = 0; i < requiredCoins.Length; i++)
            {
                requiredCoins[i].text = coins[i].ToString();
                requiredCoins[i].color = GetPurchaseStatusColor(selectMonster.data.id, i);
            }
        }
    }

    // 구매 가능한지 확인 및 설정 Color 반환

    private Color GetPurchaseStatusColor(int id, int idx)
    {
        int[] coins = evolutionResuiredCoinsDict[id];
        if (StageManager.Instance.CurrGold >= coins[idx]) return Color.white;
        else return Color.gray;
    }

    // 진화 전 몬스터 설명 띄우기
    private void DescriptionMonsterEvolution(EvolutionType evolutionType)
    {
        MonsterEvolution(evolutionType);
    }

    // 진화 (프리팹 변경)
    private void MonsterEvolution(EvolutionType evolutionType)
    {
        if (selectMonster == null) return;
        var evolution = MonsterDataManager.Instance.GetEvolutionData(selectMonster.data.id, selectMonster.data.currentLevel + 1, evolutionType);
        if (evolution != null && StageManager.Instance.CurrGold >= evolution.requiredCoins)
        {
            StageManager.Instance.ChangeGold(-evolution.requiredCoins);

            string evolutionMonsterName = GetMonsterEvolutionName(evolutionType);
            float fatigue = selectMonster.data.currentFatigue;
            Vector3 pos = selectMonster.gameObject.transform.position;
            PoolManager.Instance.ReturnToPool(selectMonster.gameObject.name, selectMonster.gameObject);
            GameObject evolutionMonster = PoolManager.Instance.SpawnFromPool(evolutionMonsterName, pos, Quaternion.identity);
            Monster monster = evolutionMonster.GetComponent<Monster>();
            monster.Evolution(MonsterDataManager.Instance.GetEvolutionData(monster.data.id, monster.data.maxLevel, evolutionType));
            
            // SO로 변경되면 추가하기
            //monster.SetMonsterDataToMonsterData(GetMonsterEvolutionData(evolutionType).data);
            evolutionUI.gameObject.SetActive(false);
        }
    }

    // 진화 프리팹 tag 가져오기
    private string GetMonsterEvolutionName(EvolutionType evolutionType)
    {
        for (int i = 0; i < evolutionMonsterList.Count; i+=2)
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
}
