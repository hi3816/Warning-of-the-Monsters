using DataTable;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterEvolutionUI : MonoBehaviour, ISell, IManagebleUI
{
    [Header("UI")]
    [SerializeField] private GameObject _evolutionUI;
    [SerializeField] private Image[] _typeImages;
    [SerializeField] private TextMeshProUGUI[] _requiredCoins;
    [SerializeField] private Button _typeButtonA;
    [SerializeField] private Button _typeButtonB;
    [SerializeField] private EvolutionStatUI _evolutionStatUI;
    [SerializeField] private GameObject _typeACheck;
    [SerializeField] private GameObject _typeBCheck;
    [SerializeField] private Button _sellButton;
    [SerializeField] private TextMeshProUGUI _sellText;

    private MonsterEvolution _monsterEvolution;
    private EvolutionType _clickEvolutionType;
    private MonsterUIManager _monsterUIManager;
    public Monster selectMonster { get; private set; } // 현재 클릭한 몬스터

    private void Awake()
    {
        _monsterUIManager = MonsterUIManager.Instance;
        _monsterEvolution = GetComponent<MonsterEvolution>();
        _typeButtonA.onClick.AddListener(() => _monsterEvolution.Evolution(selectMonster, EvolutionType.Atype));
        _typeButtonB.onClick.AddListener(() => _monsterEvolution.Evolution(selectMonster, EvolutionType.Btype));
        _typeButtonA.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Atype));
        _typeButtonB.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Btype));
        _sellButton.onClick.AddListener(SellMonster);
        _sellButton.onClick.AddListener(Hide);

        StageManager.Instance.SetMonsterUI(null, this);
        StageManager.Instance.OnChangeGold += Show;
    }

    public void Show(Monster monster)
    {
        _evolutionStatUI.gameObject.SetActive(false);
        selectMonster = monster;
        Vector3 worldPosition = monster.transform.position;
        _evolutionUI.transform.position = worldPosition;
        _evolutionUI.SetActive(true);
        SetMonsterStatPosition();
        ResetEvolutionPanel();
        SetEvolutionPanel();
    }

    public void Show()
    {
        if (selectMonster == null && !_evolutionUI.activeSelf) return;
        Show(selectMonster);
    }

    public void Hide()
    {
        if (!_evolutionUI.activeSelf) return;
        _evolutionStatUI.Hide();
        _typeACheck.SetActive(false);
        _typeBCheck.SetActive(false);
        selectMonster = null;
        _evolutionUI.SetActive(false);
    }

    private void ResetEvolutionPanel()
    {
        ResetEvolutionImageSprite();
        ResetRequiredCoinsText();
        ResetSellCoinsText();
    }

    private void SetEvolutionPanel()
    {
        SetEnvolutionImageSprite();
        SetRequiredCoinsText();
        SetSellCoinsText();
    }

    // 진화 sprite 초기화
    private void ResetEvolutionImageSprite()
    {
        foreach (Image image in _typeImages)
        {
            image.sprite = null;
        }
    }

    private void ResetRequiredCoinsText()
    {
        foreach (TextMeshProUGUI text in _requiredCoins)
        {
            text.text = null;
        }
    }

    private void ResetSellCoinsText()
    {
        _sellText.text = null;
    }

    // 진화 이미지 설정
    private void SetEnvolutionImageSprite()
    {
        Sprite[] sprites = _monsterEvolution.GetvolutionSpriteDict(selectMonster.data.id);

        for (int i = 0; i < _typeImages.Length; i++)
        {
            _typeImages[i].sprite = sprites[i];
            _typeImages[i].color = GetPurchaseStatusColor(selectMonster.data.id, i);
        }
    }

    private void SetRequiredCoinsText()
    {
        int[] coins = _monsterEvolution.GetEvolutionRequiredCoinsDict(selectMonster.data.id);

        for (int i = 0; i < _requiredCoins.Length; i++)
        {
            _requiredCoins[i].text = coins[i].ToString();
            _requiredCoins[i].color = GetPurchaseStatusColor(selectMonster.data.id, i);
        }
    }

    private void SetSellCoinsText()
    {
        _sellText.text = Mathf.RoundToInt(CalculateTotalSpent(selectMonster) * 0.35f).ToString();
    }

    public void SellMonster()
    {
        if (selectMonster == null) return;
        int totalSpent = CalculateTotalSpent(selectMonster); //여태 얼마 사용했는지 계산
        float refundPercentage = 0.35f; // 35% 환불
        int refundAmount = Mathf.RoundToInt(totalSpent * refundPercentage);
        StageManager.Instance.ChangeGold(refundAmount); //UI에 표시
        selectMonster.ReturnToVillage();
    }

    public int CalculateTotalSpent(Monster selectedMonster) //몬스터 스폰 & 업그레이드에 사용한 비용 계산
    {
        MonsterData monsterData = selectedMonster.data;
        List<int> goldList = DataManager.Instance.GetBaseMonsterById(monsterData.id).requiredCoins;
        int totalSpent = 0;
        for (int level = 0; level <= monsterData.currentLevel; level++) //몬스터 업그레이드 비용
        {
            totalSpent += goldList[level];
        }
        if (monsterData.currentLevel == monsterData.maxLevel)
        {
            Evolution_Data evolution = DataManager.Instance.GetEvolutionData(monsterData.id, monsterData.currentLevel, monsterData.evolutionType);
            totalSpent += evolution.requiredCoins;
        }
        return totalSpent;
    }

    // 구매 가능한지 확인 및 설정 Color 반환
    private Color GetPurchaseStatusColor(int id, int idx)
    {
        int[] coins = _monsterEvolution.GetEvolutionRequiredCoinsDict(id);
        if (StageManager.Instance.CurrGold >= coins[idx]) return Color.white;
        else return Color.gray;
    }

    public bool SelectEvolutionMonster(EvolutionType evolutionType)
    {
        if (_clickEvolutionType != evolutionType) return false;
        if (!_typeACheck.activeSelf && !_typeBCheck.activeSelf) return false;
        return true;
    }

    private void MonsterEvolutionStat(EvolutionType evolutionType)
    {
        if (selectMonster == null) return;
        var evolution = DataManager.Instance.GetEvolutionData(selectMonster.data.id, selectMonster.data.currentLevel + 1, evolutionType);
        if (evolution == null) return;
        if (evolutionType == EvolutionType.Atype)
        {
            _typeACheck.SetActive(true);
            _typeBCheck.SetActive(false);
        }
        else
        {
            _typeBCheck.SetActive(true);
            _typeACheck.SetActive(false);
        }
        _evolutionStatUI.Show(selectMonster.data, evolution);
        _monsterUIManager.ShowRangeIndicator(evolution);
        _clickEvolutionType = evolutionType;
    }

    private void SetMonsterStatPosition()
    {
        Vector3 posX = selectMonster.transform.position.x > 0 ? Vector3.left : Vector3.right;
        _evolutionStatUI.transform.position = selectMonster.transform.position + (posX * 3f);
    }
}
