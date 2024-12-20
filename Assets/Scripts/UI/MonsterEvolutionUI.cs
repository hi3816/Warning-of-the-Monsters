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
    [SerializeField] private EvolutionStatUI evolutionStatUI;
    [SerializeField] private GameObject typeACheck;
    [SerializeField] private GameObject typeBCheck;

    private MonsterEvolution monsterEvolution;
    private Monster selectMonster; // 현재 클릭한 몬스터
    private EvolutionType clickEvolutionType;

    private void Awake()
    {
        monsterEvolution = GetComponent<MonsterEvolution>();
        typeButtonA.onClick.AddListener(() => monsterEvolution.Evolution(selectMonster, EvolutionType.Atype));
        typeButtonB.onClick.AddListener(() => monsterEvolution.Evolution(selectMonster, EvolutionType.Btype));
        typeButtonA.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Atype));
        typeButtonB.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Btype));
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
        evolutionStatUI.Hide();
        typeACheck.SetActive(false);
        typeBCheck.SetActive(false);
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
        Sprite[] sprites = monsterEvolution.GetvolutionSpriteDict(selectMonster.data.id);

        for (int i = 0; i < typeImages.Length; i++)
        {
            typeImages[i].sprite = sprites[i];
            typeImages[i].color = GetPurchaseStatusColor(selectMonster.data.id, i);
        }
    }

    private void SetRequiredCoinsText()
    {
        int[] coins = monsterEvolution.GetEvolutionRequiredCoinsDict(selectMonster.data.id);

        for (int i = 0; i < requiredCoins.Length; i++)
        {
            requiredCoins[i].text = coins[i].ToString();
            requiredCoins[i].color = GetPurchaseStatusColor(selectMonster.data.id, i);
        }
    }

    // 구매 가능한지 확인 및 설정 Color 반환
    private Color GetPurchaseStatusColor(int id, int idx)
    {
        int[] coins = monsterEvolution.GetEvolutionRequiredCoinsDict(id);
        if (StageManager.Instance.CurrGold >= coins[idx]) return Color.white;
        else return Color.gray;
    }

    public bool SelectEvolutionMonster(EvolutionType evolutionType)
    {
        if (clickEvolutionType != evolutionType) return false;
        if (!typeACheck.activeSelf && !typeBCheck.activeSelf) return false;
        return true;
    }

    private void MonsterEvolutionStat(EvolutionType evolutionType)
    {
        var evolution = MonsterDataManager.Instance.GetEvolutionData(selectMonster.data.id, selectMonster.data.currentLevel + 1, evolutionType);
        if (evolutionType == EvolutionType.Atype)
        {
            typeACheck.SetActive(true);
            typeBCheck.SetActive(false);
            evolutionStatUI.Show(evolution);
        }
        else
        {
            typeBCheck.SetActive(true);
            typeACheck.SetActive(false);
            evolutionStatUI.Show(evolution);
        }
        clickEvolutionType = evolutionType;
    }
}
