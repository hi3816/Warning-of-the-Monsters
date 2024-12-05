using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
        agent.ResetPath();
        agent.SetDestination(target);
        bool flag = agent.remainingDistance <= agent.stoppingDistance;
        //return agent.remainingDistance <= agent.stoppingDistance;
        return flag;
    }

    // 유닛이 대형을 정비하는 메서드
    // TODO: 인간 유닛이 몬스터 기준으로 어떻게 이동할지 로직 추가
    // 현재는 그냥 타겟 위치로 이동하는 메서드와 동일
    public bool MoveToFormationPosition(Vector3 formationPosition)
    {
        agent.ResetPath();
        agent.SetDestination(formationPosition);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EndPoint"))
        {
            StartCoroutine(ReturnHumanProcess());
        }
    }

    private IEnumerator ReturnHumanProcess()
    {
        yield return new WaitForSeconds(3.0f);
        PoolManager.Instance.ReturnToPool("Human", this.gameObject);
    }
}
