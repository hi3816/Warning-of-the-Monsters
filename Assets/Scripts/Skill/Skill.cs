using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillSO SkillSO { get; private set; }
    private Animator animator;
    private WaitForSeconds animationTime;
    private Coroutine attackCoroutine;
    private Coroutine debuffCoroutine;
    private WaitForSeconds effectDurationTime;
    private CircleCollider2D skillCollider;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private int skillIdx;

    // human 확인 리스트
    private List<GameObject> humanList = new List<GameObject>();
    private float agentOriginSpeed = 0f;

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

        skillCollider = GetComponent<CircleCollider2D>();
        effectDurationTime = new WaitForSeconds(SkillSO.duration);
    }

    public void StartSkill()
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(CoEndSkillEffect());
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
                attackCoroutine = StartCoroutine(CoEndSkillEffect());
                humanObj.GetComponent<Human>().IncreaseFear(SkillSO.power);
                break;
            
            case SkillType.Debuff:
                if (attackCoroutine != null) StopCoroutine(attackCoroutine);
                attackCoroutine = StartCoroutine(CoEndSkillEffect());
                HumanController humanController = humanObj.GetComponent<HumanController>();
                CheckSkillName(humanObj, humanController);
                if (debuffCoroutine != null) StopCoroutine(debuffCoroutine);
                StartCoroutine(CoEndDebuffSkill());
                break;
        }
    }

    private void CheckSkillName(GameObject humanObj, HumanController humanController)
    {
        if (SkillSO.skillName == SkillName.FrozenGround)
        {
            agentOriginSpeed = humanController.Agent.speed;
            humanObj.GetComponent<Human>().IncreaseFear(SkillSO.power);
            humanController.Agent.speed *= ((100 - SkillSO.percentage) / 100f);
        }
    }

    // 스킬이 끝났을 때
    private IEnumerator CoEndSkillEffect()
    {
        yield return animationTime;

        if (SkillSO.skillType == SkillType.Attack)
        {
            PoolManager.Instance.ReturnToPool(SkillSO.skillName.ToString(), gameObject);
            humanList.Clear();
        }
        else
        {
            skillCollider.enabled = false;
            spriteRenderer.enabled = false;
            if(humanList.Count == 0)
            {
                if(debuffCoroutine != null) StopCoroutine(debuffCoroutine);
                StartCoroutine(CoEndDebuffSkill());
            }
        }
    }

    private IEnumerator CoEndDebuffSkill()
    {
        yield return effectDurationTime;

        foreach (GameObject obj in humanList)
        {
            UnityEngine.AI.NavMeshAgent agent = obj.GetComponent<HumanController>().Agent;
            agent.speed = agentOriginSpeed;
        }

        spriteRenderer.enabled = true;
        skillCollider.enabled = true;
        humanList.Clear();
        PoolManager.Instance.ReturnToPool(SkillSO.skillName.ToString(), gameObject);
    }
}
