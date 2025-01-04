using DataTable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterEvolutionUI : MonoBehaviour, ISell
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
    [SerializeField] private GameObject _sellButtonCanvas;
    [SerializeField] private Button _sellButton;
    [SerializeField] private TextMeshProUGUI _sellText;

    private MonsterEvolution _monsterEvolution;
    private Monster _selectMonster; // 현재 클릭한 몬스터
    private EvolutionType _clickEvolutionType;
    private MonsterUI _monsterUI;

    private void Awake()
    {
        _monsterUI = GetComponentInParent<MonsterUI>();
        _monsterEvolution = GetComponent<MonsterEvolution>();
        _typeButtonA.onClick.AddListener(() => _monsterEvolution.Evolution(_selectMonster, EvolutionType.Atype));
        _typeButtonB.onClick.AddListener(() => _monsterEvolution.Evolution(_selectMonster, EvolutionType.Btype));
        _typeButtonA.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Atype));
        _typeButtonB.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Btype));
        _sellButton.onClick.AddListener(() => SellMonster());
        _sellButton.onClick.AddListener(() => Hide());
    }

    public void Show(Monster monster)
    {
        _selectMonster = monster;
        Vector3 worldPosition = monster.transform.position;
        _evolutionUI.transform.position = worldPosition;
        _evolutionUI.SetActive(true);
        _sellButtonCanvas.transform.position = worldPosition + Vector3.down;
        _sellButtonCanvas.SetActive(true);
        _monsterUI.ShowRangeIndicator();
        ResetEvolutionPanel();
        SetEvolutionPanel();
    }

    public void Hide()
    {
        _monsterUI.HideRangeIndicator();
        _evolutionUI.SetActive(false);
        _sellButtonCanvas.SetActive(false);
        _evolutionStatUI.Hide();
        _typeACheck.SetActive(false);
        _typeBCheck.SetActive(false);
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
        Sprite[] sprites = _monsterEvolution.GetvolutionSpriteDict(_selectMonster.data.id);

        for (int i = 0; i < _typeImages.Length; i++)
        {
            _typeImages[i].sprite = sprites[i];
            _typeImages[i].color = GetPurchaseStatusColor(_selectMonster.data.id, i);
        }
    }

    private void SetRequiredCoinsText()
    {
        int[] coins = _monsterEvolution.GetEvolutionRequiredCoinsDict(_selectMonster.data.id);

        for (int i = 0; i < _requiredCoins.Length; i++)
        {
            _requiredCoins[i].text = coins[i].ToString();
            _requiredCoins[i].color = GetPurchaseStatusColor(_selectMonster.data.id, i);
        }
    }

    private void SetSellCoinsText()
    {
        _sellText.text = Mathf.RoundToInt(CalculateTotalSpent(_selectMonster) * 0.35f).ToString();
    }

    public void SellMonster()
    {
        if (_selectMonster == null) return;
        int totalSpent = CalculateTotalSpent(_selectMonster); //여태 얼마 사용했는지 계산
        float refundPercentage = 0.35f; // 35% 환불
        int refundAmount = Mathf.RoundToInt(totalSpent * refundPercentage);
        StageManager.Instance.ChangeGold(refundAmount); //UI에 표시
        _selectMonster.gameObject.SetActive(false);
        if(_selectMonster.data.monsterType == MonsterType.Summoner)
        {
            _selectMonster.ReturnToVillage();
        }
        PoolManager.Instance.ReturnToPool(_selectMonster.data.poolTag, _selectMonster);
    }

    public int CalculateTotalSpent(Monster selectedMonster) //몬스터 스폰 & 업그레이드에 사용한 비용 계산
    {
        MonsterData monsterData = selectedMonster.data;
        int totalSpent = DataManager.Instance.GetBaseMonsterById(monsterData.id).requiredCoins; //몬스터 스폰 비용
        for (int level = 1; level <= monsterData.currentLevel; level++) //몬스터 업그레이드 비용
        {
            var upgrades = DataManager.Instance.GetUpgradeMonsters(monsterData.id, level);
            if (upgrades.upgradeLevel > 0)
            {
                var upgradeData = upgrades;
                totalSpent += upgradeData.requiredCoins;
            }
        }
        // 혹시 진화 못했을 경우 대비
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
        var evolution = DataManager.Instance.GetEvolutionData(_selectMonster.data.id, _selectMonster.data.currentLevel + 1, evolutionType);
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
        _evolutionStatUI.Show(evolution);
        _monsterUI.ShowRangeIndicator(evolution);
        _clickEvolutionType = evolutionType;
    }
}
