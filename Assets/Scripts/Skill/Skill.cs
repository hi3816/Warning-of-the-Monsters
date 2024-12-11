using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private string tag;

    private Animator animator;

    private Coroutine coroutine;
    private WaitForSeconds animationTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationTime = new WaitForSeconds(stateInfo.length);
        Debug.Log($"animationTime {stateInfo.length}");
    }

    public void SetTag(string skillName)
    {
        tag = skillName;
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CoEndSkill());
    }

    // 스킬이 끝났을 때
    private IEnumerator CoEndSkill()
    {
        yield return animationTime;
        PoolManager.Instance.ReturnToPool(tag, gameObject);
        gameObject.SetActive(false);
    }
}
