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
    private List<IManagebleUI> _uiList = new List<IManagebleUI>();
    private Monster _clickedMonster;
    
    public MonsterUpgradeUI monsterUpgradeUI { get; private set; }

    public MonsterEvolutionUI monsterEvolutionUI { get; private set; }
    
    private void Awake()
    {
        base.Awake();
        _popupMonsterSpawner = GetComponentInChildren<PopupMonsterSpawner>();
        monsterUpgradeUI = GetComponentInChildren<MonsterUpgradeUI>();
        monsterEvolutionUI = GetComponentInChildren<MonsterEvolutionUI>();
        _uiList.Add(_popupMonsterSpawner);
        _uiList.Add(monsterUpgradeUI);
        _uiList.Add(monsterEvolutionUI);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 게임오브젝트가 있을 때
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int interactionLayer = LayerMask.GetMask("InteractionLayer");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, interactionLayer);

            if (hit.collider != null)
            {
                _clickedMonster = hit.collider.GetComponentInParent<Monster>();

                // 현재 배치된 몬스터가 있을 때
                if (_clickedMonster != null)
                {
                    ShowUpgradeOrEvolutionUI();

                    if (_clickedMonster is summonerMonster summoner)
                    {
                        summoner.OnMoveMode?.Invoke();
                    }
                }
            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                // 스폰 포인트일 때
                if (_popupMonsterSpawner.SearchSpawanPoint(mousePosition))
                {
                    HideRangeIndicator();
                    _popupMonsterSpawner.Show();
                    HideOtherUI(_popupMonsterSpawner);
                    return;    
                }

                HideAllMonsterUI();
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
                monsterEvolutionUI.Show(_clickedMonster);
                ShowRangeIndicator();
                HideOtherUI(monsterEvolutionUI);
            }
            else
            {
                if (_clickedMonster.data.currentLevel <= _clickedMonster.data.maxLevel)
                {
                    monsterUpgradeUI.Show(_clickedMonster);
                    ShowRangeIndicator();
                    HideOtherUI(monsterUpgradeUI);
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

        if (_clickedMonster != null && _clickedMonster is summonerMonster summoner)
        {
            summoner.OnStayMode?.Invoke();
        }
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

    public void HideAllMonsterUI()
    {
        foreach(IManagebleUI ui in _uiList)
        {
            ui.Hide();
        }
    }
}
