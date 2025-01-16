using DataTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IManagebleUI
{
    void Hide();
}

public class MonsterUIManager : SingletonBase<MonsterUIManager>
{
    [SerializeField] private GameObject _rangeIndicator;
    private PopupMonsterSpawner _popupMonsterSpawner;
    private MonsterUpgradeUI _monsterUpgradeUI;
    private MonsterEvolutionUI _monsterEvolutionUI;
    private List<IManagebleUI> _uiList = new List<IManagebleUI>();
    private Monster _clickedMonster;

    private void Awake()
    {
        base.Awake();
        _popupMonsterSpawner = GetComponentInChildren<PopupMonsterSpawner>();
        _monsterUpgradeUI = GetComponentInChildren<MonsterUpgradeUI>();
        _monsterEvolutionUI = GetComponentInChildren<MonsterEvolutionUI>();
        _uiList.Add(_popupMonsterSpawner);
        _uiList.Add(_monsterUpgradeUI);
        _uiList.Add(_monsterEvolutionUI);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // When player clicks
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int interactionLayer = LayerMask.GetMask("InteractionLayer");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, interactionLayer);

            if (hit.collider != null)
            {
                _clickedMonster = hit.collider.GetComponentInParent<Monster>();

                if (_clickedMonster != null)
                {
                    _clickedMonster.OnHideMonsterUI -= _monsterUpgradeUI.Hide;
                    _clickedMonster.OnHideMonsterUI -= _monsterEvolutionUI.Hide;
                    _clickedMonster.OnHideMonsterUI -= HideRangeIndicator;
                    _clickedMonster.OnHideMonsterUI += _monsterUpgradeUI.Hide;
                    _clickedMonster.OnHideMonsterUI += _monsterEvolutionUI.Hide;
                    _clickedMonster.OnHideMonsterUI += HideRangeIndicator;
                    ShowUpgradeOrEvolutionUI();

                    if (_clickedMonster is summonerMonster summoner)
                    {
                        summoner.OnMoveMode?.Invoke();
                    }
                }
            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (_popupMonsterSpawner.SearchSpawanPoint(mousePosition))
                {
                    _popupMonsterSpawner.Show();
                    HideOtherUI(_popupMonsterSpawner);
                    return;    
                }

                _monsterUpgradeUI.Hide();
                _monsterEvolutionUI.Hide();
                _popupMonsterSpawner.Hide();
                HideRangeIndicator();
            }
        }
    }

    private void ShowUpgradeOrEvolutionUI()
    {
        if (_clickedMonster.data.currentLevel <= _clickedMonster.data.maxLevel)
        {
            DataTable.Evolution_Data data = DataManager.Instance.GetEvolutionData(_clickedMonster.data.id, _clickedMonster.data.currentLevel + 1);
            if (data != null && data.upgradeLevel == _clickedMonster.data.maxLevel)
            {
                _monsterEvolutionUI.Show(_clickedMonster);
                ShowRangeIndicator();
                HideOtherUI(_monsterEvolutionUI);
            }
            else
            {
                if (_clickedMonster.data.currentLevel <= _clickedMonster.data.maxLevel)
                {
                    _monsterUpgradeUI.Show(_clickedMonster);
                    ShowRangeIndicator();
                    HideOtherUI(_monsterUpgradeUI);
                }
            }
        }
    }

    public void ShowRangeIndicator()
    {
        if (_clickedMonster == null || _rangeIndicator == null) return;
        float range = _clickedMonster.data.humanDetectRange;
        _rangeIndicator.transform.localScale = Vector2.one * range;
        _rangeIndicator.transform.position = _clickedMonster.transform.position;
        _rangeIndicator.SetActive(true);
    }

    public void ShowRangeIndicator(DataTable.Evolution_Data evolution)
    {
        if (evolution == null || _rangeIndicator == null) return;
        float range = evolution.humanScaringRange; // humanScaringRange == humanDetactRange
        _rangeIndicator.transform.localScale = Vector2.one * range;
        _rangeIndicator.transform.position = _clickedMonster.transform.position;
        _rangeIndicator.SetActive(true);
    }

    public void ShowRangeIndicator(Vector3 pos, float humanDetactRange)
    {
        if (_rangeIndicator == null) return;
        float range = humanDetactRange;
        _rangeIndicator.transform.localScale = Vector2.one * range;
        _rangeIndicator.transform.position = pos;
        _rangeIndicator.SetActive(true);
    }

    public void HideRangeIndicator()
    {
        if (_rangeIndicator == null) return;
        _rangeIndicator.SetActive(false);
    }

    private void HideOtherUI(IManagebleUI showUI)
    {
        foreach(IManagebleUI ui in _uiList)
        {
            if (ui != showUI)
            {
                ui.Hide();
            }
        }
    }
}
