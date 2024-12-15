using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterEvolutionUI : MonoBehaviour
{
    // Inspector에서 Sprite 넣기 위해 사용하는 List
    [SerializeField] private Image[] typeImages;
    [SerializeField] private GameObject evolutionUI;

    private Dictionary<float, Sprite[]> evolutionSpriteDict = new Dictionary<float, Sprite[]>(); // 진화 스프라이트 담을 Dict

    private Monster selectMonster; // 현재 클릭한 몬스터의 Monster 클래스
    private GameObject selectMonsterObj;

    [SerializeField] private PoolManager.PoolConfig[] poolConfigs;
    [SerializeField] private List<Monster> poolConfigMonsterList;

    private StageManager stageManager;

    [SerializeField] private Button typeButtonA;
    [SerializeField] private Button typeButtonB;

    private void Awake()
    {
        stageManager = StageManager.Instance;
        PoolManager.Instance.AddPoolS(poolConfigs);
        SetSprite();
        typeButtonA.onClick.AddListener(() => MonsterEvolution(EvolutionType.Atype));
        typeButtonB.onClick.AddListener(() => MonsterEvolution(EvolutionType.Btype));
    }

    private void Start()
    {
        // DataManager.Instance.GetEvolutionData가 Awake에서는 동작 안함
        SetEvolutionData();
    }

    private void SetSprite()
    {
        // 로비씬에서 선택한 4개의 몬스터
        var selectedMonsterData = DataManager.Instance.SelectedMonsterData;

        for (int i = 0; i < poolConfigs.Length; i+=2)
        {
            Monster monster = poolConfigs[i].prefab.GetComponent<Monster>();
            int monsterID = monster.data.Id;

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
        for (int i = 0; i < poolConfigs.Length; i+=2)
        {
            Monster monster1 = poolConfigs[i].prefab.GetComponent<Monster>();
            Monster monster2 = poolConfigs[i + 1].prefab.GetComponent<Monster>();
            monster1.Evolution(MonsterDataManager.Instance.GetEvolutionData(monster1.data.Id, monster1.data.MaxLevel, EvolutionType.Atype));
            monster2.Evolution(MonsterDataManager.Instance.GetEvolutionData(monster2.data.Id, monster2.data.MaxLevel, EvolutionType.Btype));
            poolConfigMonsterList.Add(monster1);
            poolConfigMonsterList.Add(monster2);
        }
    }

    public void Show(Monster monster)
    {
        selectMonster = monster;
        selectMonsterObj = monster.gameObject;
        Vector3 worldPosition = monster.transform.position;
        evolutionUI.transform.position = worldPosition + Vector3.up;
        evolutionUI.SetActive(true);
        ResetEvolutionImageSprite();
        SetEnvolutionImageSprite();
    }

    public void Hide()
    {
        evolutionUI.SetActive(false);
    }

    // 진화 sprite 초기화
    private void ResetEvolutionImageSprite()
    {
        foreach(Image image in typeImages)
        {
            image.sprite = null;
        }
    }

    // 진화 이미지 설정
    public void SetEnvolutionImageSprite()
    {
        if (evolutionSpriteDict.ContainsKey(selectMonster.data.Id))
        {
            Sprite[] sprites = evolutionSpriteDict[selectMonster.data.Id];

            for (int i = 0; i < typeImages.Length; i++)
            {
                typeImages[i].sprite = sprites[i];
            }
        }
    }

    // 진화 (프리팹 변경)
    private void MonsterEvolution(EvolutionType evolutionType)
    {
        if (selectMonster == null) return;
        var evolution = MonsterDataManager.Instance.GetEvolutionData(selectMonster.data.Id, selectMonster.data.CurrentLevel + 1, evolutionType);
        if (evolution != null && stageManager.CurrGold >= evolution.requiredCoins)
        {
            stageManager.ChangeGold(-evolution.requiredCoins);
            string evolutionMonsterName = GetMonsterEvolutionName(evolutionType);
            PoolManager.Instance.SpawnFromPool(evolutionMonsterName, selectMonster.transform.position, Quaternion.identity).SetActive(true);
            PoolManager.Instance.ReturnToPool(selectMonster.gameObject.name, selectMonsterObj);
            evolutionUI.gameObject.SetActive(false);
        }
        else
        {
            print("Not enough gold to upgrade!");
        }
    }

    private string GetMonsterEvolutionName(EvolutionType evolutionType)
    {
        for (int i = 0; i < poolConfigs.Length; i += 2)
        {
            Monster monster = poolConfigMonsterList[i];
            Monster monster2 = poolConfigMonsterList[i + 1];
            int monsterID = monster.data.Id;

            if (monsterID == selectMonster.data.Id)
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
