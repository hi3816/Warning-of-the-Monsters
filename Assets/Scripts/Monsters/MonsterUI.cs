using DataTable;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] private MonsterUpgradeUI _monsterUpgradeUI;
    [SerializeField] private MonsterEvolutionUI _monsterEvolutionUI;
    [SerializeField] private GameObject _rangeIndicator;

    private Monster _clickedMonster;

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
                _clickedMonster.OnHideMonsterUI -= _monsterUpgradeUI.Hide;
                _clickedMonster.OnHideMonsterUI -= _monsterEvolutionUI.Hide;
                _clickedMonster.OnHideMonsterUI -= HideRangeIndicator;
                _clickedMonster.OnHideMonsterUI += _monsterUpgradeUI.Hide;
                _clickedMonster.OnHideMonsterUI += _monsterEvolutionUI.Hide;
                _clickedMonster.OnHideMonsterUI += HideRangeIndicator;

                if (_clickedMonster != null)
                {
                    ShowUpgradeOrEvolutionUI();
                    _clickedMonster.OnPositionMode?.Invoke();
                }
            }
            else if(!EventSystem.current.IsPointerOverGameObject())
            {
                _monsterUpgradeUI.Hide();
                _monsterEvolutionUI.Hide();
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
            }
            else
            {
                if (_clickedMonster.data.currentLevel <= _clickedMonster.data.maxLevel)
                {
                    _monsterUpgradeUI.Show(_clickedMonster);
                    ShowRangeIndicator();
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

    public void HideRangeIndicator()
    {
        _rangeIndicator.SetActive(false);
    }
}
