using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterUpgrade : MonoBehaviour
{
    [SerializeField] private MonsterUpgradeUI monsterUpgradeUI;
    [SerializeField] private MonsterEvolutionUI evolutionUI;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // When player clicks
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int interactionLayer = LayerMask.GetMask("InteractionLayer");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, interactionLayer);

            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
                Monster clickedMonster = hit.collider.GetComponentInParent<Monster>();
                if (clickedMonster != null)
                {
                    Debug.Log($"ClickedMonster MonsterID: {clickedMonster.data.monsterId}");
                    if (clickedMonster.currentUpgradeLevel <= clickedMonster.data.maxLevel)
                    {
                        monsterUpgradeUI.Show(clickedMonster);
                    }
                    else
                    {
                        evolutionUI.Show(clickedMonster);
                    }
                }
            }
            else if(!EventSystem.current.IsPointerOverGameObject())
            {
                monsterUpgradeUI.Hide();
                evolutionUI.Hide();
            }
        }
    }
}
