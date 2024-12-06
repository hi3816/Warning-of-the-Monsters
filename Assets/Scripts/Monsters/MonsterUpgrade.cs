using UnityEngine;

public class MonsterUpgrade : MonoBehaviour
{
    MonsterUpgradeUI monsterUpgradeUI;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Monster"))
            {
                Monster clickedMonster = hit.collider.GetComponent<Monster>();
                if (clickedMonster != null)
                {
                    monsterUpgradeUI.Show(clickedMonster);
                }
            }
        }
    }
}
