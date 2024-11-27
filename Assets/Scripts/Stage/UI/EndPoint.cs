using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // HumanSO.lifeInflicted °ªÀ¸·Î ±ï±â
            // StageManager.Instance.ChangeHealth(humanSO.lifeInflicted);
            StageManager.Instance.ChangeHealth(-1);
            Destroy(collision.gameObject);
            Debug.LogAssertion("Enemy Collision");
        }
    }
}
