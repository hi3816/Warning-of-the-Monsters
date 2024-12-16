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

    [Header("Pool")]
    [SerializeField] private PoolManager.PoolConfig[] poolConfigs;
    [SerializeField] private List<Monster> poolConfigMonsterList;

    // 진화 스프라이트, 필요재화 담을 Dictionary
    private Dictionary<int, Sprite[]> evolutionSpriteDict = new Dictionary<int, Sprite[]>();
    private Dictionary<int, int[]> evolutionResuiredCoinsDict = new Dictionary<int, int[]>();

    private Monster selectMonster; // 현재 클릭한 몬스터

    private void Awake()
    {
        SetSprite();
        typeButtonA.onClick.AddListener(() => MonsterEvolution(EvolutionType.Atype));
        typeButtonB.onClick.AddListener(() => MonsterEvolution(EvolutionType.Btype));
    }

    private void Start()
    {
        // DataManager.Instance.GetEvolutionData가 Awake에서는 동작 안함
        SetEvolutionData();
        PoolManager.Instance.AddPoolS(poolConfigs);
    }

    private void SetSprite()
    {
        // 로비씬에서 선택한 4개의 몬스터
        var selectedMonsterData = DataManager.Instance.SelectedMonsterData;

        for (int i = 0; i < poolConfigs.Length; i+=2)
        {
            Monster monster = poolConfigs[i].prefab.GetComponent<Monster>();
            int monsterID = monster.data.id;

            foreach (int key in selectedMonsterData.Keys)
            {
                if(monsterID == selectedMonsterData[key].Item1)
                {
                    Sprite sprite1 = poolConfigs[i].prefab.GetComponent<SpriteRenderer>().sprite;
                    Sprite sprite2 = poolConfigs[i+1].prefab.GetComponent<SpriteRenderer>().sprite;
                    Sprite[] spriteArr = { sprite1, sprite2 };

                    if (!evolutionSpriteDict.ContainsKey(monsterID))
                    {
                        evolutionSpriteDict.Add(monsterID, spriteArr);
                    }
                }
            }
        }
    }

    private void SetEvolutionData()
    {
        for (int i = 0; i < poolConfigs.Length; i += 2)
        {
            Monster monster1 = poolConfigs[i].prefab.GetComponent<Monster>();
            Monster monster2 = poolConfigs[i + 1].prefab.GetComponent<Monster>();
            monster1.Evolution(MonsterDataManager.Instance.GetEvolutionData(monster1.data.id, monster1.data.maxLevel, EvolutionType.Atype));
            monster2.Evolution(MonsterDataManager.Instance.GetEvolutionData(monster2.data.id, monster2.data.maxLevel, EvolutionType.Btype));
            poolConfigMonsterList.Add(monster1);
            poolConfigMonsterList.Add(monster2);

            int[] requiredCoins = { monster1.data.requiredCoins, monster2.data.requiredCoins };
            evolutionResuiredCoinsDict.Add(monster1.data.id, requiredCoins);
        }
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
            }
        }
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
            PoolManager.Instance.SpawnFromPool(evolutionMonsterName, selectMonster.gameObject.transform.position, Quaternion.identity).SetActive(true);
            PoolManager.Instance.ReturnToPool(selectMonster.gameObject.name, selectMonster.gameObject);
            evolutionUI.gameObject.SetActive(false);
        }
        else
        {
            print("Not enough gold to upgrade!");
        }
    }

    // 진화 프리팹 tag 가져오기
    private string GetMonsterEvolutionName(EvolutionType evolutionType)
    {
        for (int i = 0; i < poolConfigs.Length; i += 2)
        {
            Monster monster = poolConfigMonsterList[i];
            int monsterID = monster.data.id;

            if (monsterID == selectMonster.data.id)
            {
                if (evolutionType == EvolutionType.Atype)
                {
                    return poolConfigs[i].tag;
                }
                else if (evolutionType == EvolutionType.Btype)
                {
                    return poolConfigs[i + 1].tag;
                }
            }
        }
        return null;
    }
}
