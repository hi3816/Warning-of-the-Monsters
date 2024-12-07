using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            // HumanSO.lifeInflicted 값으로 깍기
            int lifeInflicted = collision.gameObject.GetComponent<Human>().LifeInflicted;
            StageManager.Instance.ChangeHealth(-lifeInflicted);
            collision.gameObject.GetComponent<HumanController>().ReturnHumanToPool(0.5f);
            // Debug.Log("Enemy Collision");
        }
    }
}
