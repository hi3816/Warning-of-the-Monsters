using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterUpgrade : MonoBehaviour
{
    [SerializeField] private MonsterUpgradeUI _monsterUpgradeUI;
    [SerializeField] private MonsterEvolutionUI _evolutionUI;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // When player clicks
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int interactionLayer = LayerMask.GetMask("InteractionLayer");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, interactionLayer);

            if (hit.collider != null)
            {
                Monster clickedMonster = hit.collider.GetComponentInParent<Monster>();

                if (clickedMonster != null)
                {
                    ShowUpgradeOrEvolutionUI(clickedMonster);
                }
            }
            else if(!EventSystem.current.IsPointerOverGameObject())
            {
                _monsterUpgradeUI.Hide();
                _evolutionUI.Hide();
            }
        }
    }

    private void ShowUpgradeOrEvolutionUI(Monster clickedMonster)
    {
        if (clickedMonster.data.currentLevel <= clickedMonster.data.maxLevel)
        {
            EvolutionSO data = DataManager2.Instance.GetEvolutionSO(clickedMonster.data.id, clickedMonster.data.currentLevel + 1);
            if (data != null && data.upgradeLevel == clickedMonster.data.maxLevel)
            {
                _evolutionUI.Show(clickedMonster);
            }
            else
            {
                if (clickedMonster.data.currentLevel <= clickedMonster.data.maxLevel)
                {
                    _monsterUpgradeUI.Show(clickedMonster);
                }
            }
        }
    }
}
