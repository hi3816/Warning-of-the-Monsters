using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Skill : MonoBehaviour
{
    private SkillSO skillSO;
    private Animator animator;
    private WaitForSeconds animationTime;
    private Coroutine coroutine;
    private SpriteRenderer spriteRenderer;

    // human 확인 리스트
    private List<GameObject> humansList = new List<GameObject>();

    private void Awake()
    {
        // 일단 0번으로 설정 추후에 SkillButtonController를 SkillManager로 바꿀지 논의
        skillSO = DataManager.Instance.GetSkillByIndex(0);
        SetComponent();
    }

    // 컴포넌트 캐싱
    private void SetComponent()
    {
        // 애니메이션 길이 저장
        animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationTime = new WaitForSeconds(stateInfo.length);
        
        // 스킬 범위와 renderer 사이즈 동기화
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(skillSO.range, skillSO.range);
    }

    // 스킬 계산 시작
    public void StartSkill()
    {
        if (humansList != null) humansList.Clear(); 
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            // 이미 스킬에 맞았으면 계산하지 않기
            if (humansList.Contains(collision.gameObject)) return;

            // 리스트에 없으면 추가하고 스킬 공격력 만큼 피해 입히기
            humansList.Add(collision.gameObject);
            collision.gameObject.GetComponent<Human>().IncreaseFear(skillSO.power);
        }
    }
}
