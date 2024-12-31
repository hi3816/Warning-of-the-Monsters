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

                if (_clickedMonster != null)
                {
                    ShowUpgradeOrEvolutionUI();
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
            }
            else
            {
                if (_clickedMonster.data.currentLevel <= _clickedMonster.data.maxLevel)
                {
                    _monsterUpgradeUI.Show(_clickedMonster);
                }
            }
        }
    }

    public void ShowRangeIndicator()
    {
        if (_clickedMonster == null || _rangeIndicator == null) return;
        float range = _clickedMonster.data.humanScaringRange;
        _rangeIndicator.transform.localScale = new Vector3(range, range, 1);
        _rangeIndicator.transform.position = _clickedMonster.transform.position;
        _rangeIndicator.SetActive(true);
    }

    public void ShowRangeIndicator(DataTable.Evolution_Data evolution)
    {
        if (evolution == null || _rangeIndicator == null) return;
        float range = evolution.humanScaringRange;
        _rangeIndicator.transform.localScale = new Vector3(range, range, 1);
        _rangeIndicator.transform.position = _clickedMonster.transform.position;
        _rangeIndicator.SetActive(true);
    }

    public void HideRangeIndicator()
    {
        _rangeIndicator.SetActive(false);
    }
}
