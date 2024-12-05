using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // HumanSO.lifeInflicted 값으로 깍기
            // StageManager.Instance.ChangeHealth(humanSO.lifeInflicted);
            // Destroy(collision.gameObject);
            Debug.LogAssertion("Enemy Collision");
        }
    }
}
