using GoogleSheet.Type;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skill : MonoBehaviour
{
    public SkillSO SkillSO { get; private set; } 
    private Animator animator;
    private WaitForSeconds animationTime;
    private Coroutine attackCoroutine;
    private Coroutine debuffCoroutine;
    private WaitForSeconds effectDurationTime;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D skillCollider;
    [SerializeField] int skillIdx;

    // human 확인 리스트
    private List<GameObject> humanList = new List<GameObject>();

    private void Awake()
    {
        SkillSO = DataManager.Instance.GetSkillByIndex(skillIdx);
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
        spriteRenderer.size = new Vector2(SkillSO.range, SkillSO.range);

        effectDurationTime = new WaitForSeconds(SkillSO.duration);
        Debug.Log($"effectDurationTime{SkillSO.duration}");

        skillCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            // 이미 스킬에 맞았으면 계산하지 않기
            if (humanList.Contains(collision.gameObject)) return;

            // 리스트에 없으면 추가하고 스킬 공격력 만큼 피해 입히기
            humanList.Add(collision.gameObject);
            CheckSkillType(collision.gameObject);
        }
    }

    private void CheckSkillType(GameObject humanObj)
    {
        switch (SkillSO.skillType)
        {
            case SkillType.Attack:
                if (attackCoroutine != null) StopCoroutine(attackCoroutine);
                attackCoroutine = StartCoroutine(CoEndAtaackSkill());
                humanObj.GetComponent<Human>().IncreaseFear(SkillSO.power);
                break;
            
            case SkillType.Debuff:

                NavMeshAgent agent = humanObj.GetComponent<HumanController>().Agent;
                humanObj.GetComponent<Human>().IncreaseFear(SkillSO.power);
                spriteRenderer.enabled = false;
                skillCollider.enabled = false;
                
                if (debuffCoroutine != null) StopCoroutine(debuffCoroutine);
                debuffCoroutine = StartCoroutine(CoEndDebuffSkill(agent.speed));
                
                agent.speed *= ((100f - SkillSO.percentage) / 100f);
                break;
        }
    }

    // 스킬이 끝났을 때
    private IEnumerator CoEndAtaackSkill()
    {
        yield return animationTime;
        PoolManager.Instance.ReturnToPool(SkillSO.skillName.ToString(), gameObject);
        humanList.Clear();
        gameObject.SetActive(false);
    }

    private IEnumerator CoEndDebuffSkill(float originSpeed)
    {
        yield return effectDurationTime;
        foreach(GameObject obj in humanList)
        {
            NavMeshAgent agent = obj.GetComponent<HumanController>().Agent;
            agent.speed = originSpeed;
            Debug.Log($"EndDebuffSkill agent.speed {agent.speed}");
        }

        spriteRenderer.enabled = true;
        skillCollider.enabled = true;
        PoolManager.Instance.ReturnToPool(SkillSO.skillName.ToString(), gameObject);
        gameObject.SetActive(false);
        humanList.Clear();
    }
}
