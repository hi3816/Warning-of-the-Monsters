using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterEvolutionUI : MonoBehaviour
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

    private MonsterEvolution _monsterEvolution;
    private Monster _selectMonster; // 현재 클릭한 몬스터
    private EvolutionType _clickEvolutionType;

    private void Awake()
    {
        _monsterEvolution = GetComponent<MonsterEvolution>();
        _typeButtonA.onClick.AddListener(() => _monsterEvolution.Evolution(_selectMonster, EvolutionType.Atype));
        _typeButtonB.onClick.AddListener(() => _monsterEvolution.Evolution(_selectMonster, EvolutionType.Btype));
        _typeButtonA.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Atype));
        _typeButtonB.onClick.AddListener(() => MonsterEvolutionStat(EvolutionType.Btype));
    }

    private int GetIdByType(Monster monster)
    {
        if (monster.data.monsterType == MonsterType.Minion) return monster.data.summonerId;
        return monster.data.id;
    }

    public void Show(Monster monster)
    {
        _selectMonster = monster;
        Vector3 worldPosition = monster.transform.position;
        _evolutionUI.transform.position = worldPosition + Vector3.up;
        _evolutionUI.SetActive(true);
        ResetEvolutionPanel();
        SetEvolutionPanel();
    }

    public void Hide()
    {
        _evolutionUI.SetActive(false);
        _evolutionStatUI.Hide();
        _typeACheck.SetActive(false);
        _typeBCheck.SetActive(false);
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
        if (evolutionType == EvolutionType.Atype)
        {
            _typeACheck.SetActive(true);
            _typeBCheck.SetActive(false);
            _evolutionStatUI.Show(evolution);
        }
        else
        {
            _typeBCheck.SetActive(true);
            _typeACheck.SetActive(false);
            _evolutionStatUI.Show(evolution);
        }
        _clickEvolutionType = evolutionType;
    }
}
