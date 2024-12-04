using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HumanController : MonoBehaviour
{
    public Human human;
    public Animator animator;
    private NavMeshAgent agent;
    public bool IsAttacked;

    private float lastAttackTime;

    private void Awake()
    {
        human = GetComponent<Human>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();  // 자식 오브젝트(MainSprite)의 애니메이터 가져오기
        if (animator == null)
        {
            Debug.LogAssertion("Human Animator not found");
        }
    }

    public bool ArriveToDestination(Vector3 target)
    {
        agent.SetDestination(target);
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    // 유닛이 대형을 정비하는 메서드
    // TODO: 인간 유닛이 몬스터 기준으로 어떻게 이동할지 로직 추가
    // 현재는 그냥 타겟 위치로 이동하는 메서드와 동일
    public bool MoveToFormationPosition(Vector3 formationPosition)
    {
        agent.ResetPath();
        // formationPosition = agent.transform.position;   // TestCode: SetFormation -> AttackSquence로 바로 전환
        // return agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("IsSet", true);
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
        //lastAttackTime = Time.time; // 마지막 공격 시각 갱신
        //if (human.targetMonster == null) return;
        //if (human.targetMonster != null)    // 타겟으로 하는 몬스터 있으면
        {
            // 몬스터 피로도를 최소 ~ 최대 피로도 영향 수치 내에서 랜덤 값으로 증가
            // TODO: Monster.cs에서 접근 제한 수정되면 주석 해제
            animator.SetTrigger("Attack");
            //Debug.LogAssertion("Human attacked Monster");
            /* TestCode */
            // 공격 여러번하면 몬스터 피로도 올라가고 20(임의의 값) 이상이면 타겟 몬스터 null로 만들기
           // human.targetMonster.CurrentFatigue += Random.Range(human.humanData.minFatigueInflicted, human.humanData.maxFatigueInflicted);
           //  Debug.LogAssertion($"MonsterFatigue:{human.targetMonster.CurrentFatigue}");
           //  if (human.targetMonster.CurrentFatigue >= 20)
           //      human.targetMonster = null;
           float randValue = Random.Range(human.humanData.minFatigueInflicted, human.humanData.maxFatigueInflicted);
           human.targetMonster.IncreaseFatigue(randValue);
           lastAttackTime = Time.time;
        }
    }

    // 몬스터가 공격범위 내에 있는지 확인
    public bool MonsterInRange(float attackRange)
    {
        return human.targetMonster != null && Vector3.Distance(transform.position, human.targetMonster.transform.position) <= attackRange;
    }

    public void SetTargetMonster(Monster monster)
    {
        human.targetMonster = monster;
    }
    
    public bool HasTargetMonster()
    {
        return human.targetMonster != null;
    }

    public bool IsWaveStarted()
    {
        return human.IsWaveStarted;
    }

    // 공포 수치 증가시키는 메서드
    public void IncreaseFear(float amount)
    {
        human.FearLevel += amount;
        Debug.LogWarning($"Fear: {human.FearLevel}");
        // 최대 공포 수치 넘지 않도록 조정
        if (IsFearMaxed())
        {
            human.FearLevel = human.humanData.maxFear;
        }
    }

    // 최대 공포 수치 넘었는지 확인
    public bool IsFearMaxed()
    {
        return human.FearLevel >= human.humanData.maxFear;
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
}
