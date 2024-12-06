using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class HumanController : MonoBehaviour
{
    public Human human;
    public Animator animator;
    private NavMeshAgent agent;
    private Vector3 oldTarget;
    public bool IsAttacked;
    private float maxFear;

    private float lastAttackTime;
    private bool isReturning = false;
    
    public TextMeshProUGUI nodeTxt;
    public Image fearGauge;
    
    private void Awake()
    {
        human = GetComponent<Human>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();  // 자식 오브젝트(MainSprite)의 애니메이터 가져오기
        if (animator == null)
        {
            Debug.LogAssertion("Human Animator not found");
        }
        maxFear = human.humanData.maxFear;
    }

    private void Update()
    {
        fearGauge.fillAmount = human.FearLevel / maxFear;
    }

    public bool ArriveToDestination(Vector3 target)
    {
        if (oldTarget != target)
        {
            oldTarget = target;
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(target, path);
            agent.SetPath(path);
        }

        bool flag = agent.hasPath;
        if (!flag)
            StartCoroutine(ReturnHumanProcess());
        return flag;
    }

    // 유닛이 대형을 정비하는 메서드
    // TODO: 인간 유닛이 몬스터 기준으로 어떻게 이동할지 로직 추가
    // 현재는 그냥 타겟 위치로 이동하는 메서드와 동일
    public bool MoveToFormationPosition(Vector3 formationPosition)
    {
        agent.ResetPath();
        agent.SetDestination(formationPosition);
        return false;
    }

    // 현재 방향에서 몬스터 방향으로 바라보게 하는 메서드(에임 맞추기)
    // TODO: 대형 맞춘 위치 기준으로 방향 설정
    public void RotateTowardsMonster(Vector3 monsterPosition)
    {
    }

    // 공격 쿨타임 다 채워졌는지 확인
    public bool CanAttack()
    {
        return human.targetMonster != null && Time.time - lastAttackTime > human.humanData.cooldown;
    }

    // 몬스터 공격하는 메서드
    public void PerformAttack()
    {
        animator.SetTrigger("Attack");
        float randValue = Random.Range(human.humanData.minFatigueInflicted, human.humanData.maxFatigueInflicted);
        human.targetMonster.IncreaseFatigue(randValue);
        lastAttackTime = Time.time;
    }

    public void SetTargetMonster(Monster monster)
    {
        human.targetMonster = monster;
    }
    
    public bool HasTargetMonster()
    {
        bool hasTarget = human.targetMonster != null;
        if (!hasTarget)
            animator.SetBool("IsBattle", false);
        return hasTarget;
    }

    public bool IsWaveStarted()
    {
        return human.IsWaveStarted;
    }

    // 공포 수치 증가시키는 메서드
    public void IncreaseFear(float amount)
    {
        animator.SetTrigger("Surprise");
        SoundManager.Instance.PlaySFX(SfxType.SurprisingHuman);
        //nodeTxt.text = "Surprise";

        human.FearLevel += amount;
        Debug.LogWarning($"Fear: {human.FearLevel}");
        //fearGauge.fillAmount = human.FearLevel / maxFear;
        // 최대 공포 수치 넘지 않도록 조정
        if (IsFearMaxed())
        {
            human.FearLevel = maxFear;
        }
    }

    // 최대 공포 수치 넘었는지 확인
    public bool IsFearMaxed()
    {
        bool isFearMaxed = human.FearLevel >= human.humanData.maxFear;
        if (isFearMaxed)
            StartCoroutine(ReturnHumanProcess());
        return isFearMaxed;
    }

    // 몬스터의 겁주기에 반응하는 메서드(피격 로직)
    public void ReactToScaring()
    {
        if (human.targetMonster != null)
        {
            Debug.Log("ReactToScaring");
            //IncreaseFear(human.targetMonster.data.fearInflicted);   // 공포 수치 증가
            // TestCode 임시로 값 부여
            IncreaseFear(10);   // 공포 수치 증가
            animator.SetTrigger("Surprise");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EndPoint"))
        {
            StartCoroutine(ReturnHumanProcess());
        }
    }
    
    private IEnumerator ReturnHumanProcess()
    {
        if (!isReturning)
        {
            Debug.Log("Returning human process");
            isReturning = true;
            yield return new WaitForSeconds(1.0f);
            PoolManager.Instance.ReturnToPool("Human", this.gameObject);
        }
    }
}
