using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private SkillSO skillSO;
    private Animator animator;
    private WaitForSeconds animationTime;
    private Coroutine coroutine;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        // 일단 0번으로 설정 추후에 SkillButtonController를 SkillManager로 바꿀지 논의
        skillSO = DataManager.Instance.GetSkillByIndex(0);
        SetComponent();
    }

    private void SetComponent()
    {
        // 애니메이션 길이 저장
        animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationTime = new WaitForSeconds(stateInfo.length);
        
        // 스킬 범위와 collider, renderer 사이즈 동기화
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = skillSO.range;
        spriteRenderer.size = new Vector2(skillSO.range, skillSO.range);
        Debug.Log($"spriteRendererSize {spriteRenderer.size} skillSORange{skillSO.range}");
    }

    public void StartSkill()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CoEndSkill());
    }

    // 스킬이 끝났을 때
    private IEnumerator CoEndSkill()
    {
        yield return animationTime;
        PoolManager.Instance.ReturnToPool(skillSO.skillName.ToString(), gameObject);
        gameObject.SetActive(false);
    }
}
