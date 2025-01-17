using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            // HumanSO.lifeInflicted 값으로 깍기
            Human human = collision.gameObject.GetComponent<Human>();
            StageManager.Instance.ChangeHealth(-human.LifeInflicted);
            human.ReturnHumanToPool(2.0f);
        }
    }
}
