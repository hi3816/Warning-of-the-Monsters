using System;
using System.Collections;
using DataTable;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    private Human_Data _humanData;
    public int id;
    private int _coin;  // 놀랐을 때 떨어뜨리고 가는 재화량(처치 시 획득 재화량)
    public float MaxFear { get; private set; }
    public float FearLevel { get; private set; }
    public int LifeInflicted { get; private set; }
    public int SpawnedWaveIdx { get; set; }
    public bool isReturning;  // 풀에 반환중인 상태인지 체크

    public HumanController controller;

    public Action OnFearChanged;

    protected virtual void Awake()
    {
        // 데이터 세팅
        _humanData = DataManager.Instance.GetHumanByIndex(id);
        MaxFear = _humanData.maxFear;
        _coin = _humanData.coin;
        LifeInflicted = _humanData.lifeInflicted;
        
        if (controller == null)
        {
            controller = gameObject.GetComponent<HumanController>();
        }
        controller.SetHumanData(_humanData); // 컨트롤러에서 사용하는 데이터 세팅
    }

    protected virtual void OnEnable()
    {
        // 활성화 시 매번 공포 수치와 UI 초기화
        FearLevel = 0;
        isReturning = false;   // 반환하고 있지 않은 상태로 전환
        OnFearChanged -= SetBattleState;
        OnFearChanged += SetBattleState;
    }

    // 인간 공포 수치 증가시키기
    public virtual void IncreaseFear(float amount)
    {
        // 놀라는 효과음과 애니메이션 실행
        SoundManager.Instance.PlaySFX(SfxType.SurprisingHuman);
        controller.animator.SetTrigger("Surprise");
        
        if (isReturning) return;    // 반환 중인 상태면 공포 수치 올리지 않고 리턴
        
        FearLevel = Mathf.Min(FearLevel + amount, MaxFear); // 최댓값 넘지 않도록 제한
        OnFearChanged?.Invoke();    // UI 갱신 & 상태 전이 검사
    }
    
    // 인간 공포 수치 감소시키기
    public void DecreaseFear(float amount)
    {
        FearLevel = Mathf.Max(FearLevel - amount, 0);   // 최솟값 벗어나지 않도록 제한
        OnFearChanged?.Invoke();
    }

    private void SetBattleState()
    {
        if (FearLevel >= MaxFear) // 갱신된 값이 최댓값보다 크면
        {
            controller.animator.SetBool("IsBattle", false);
            controller.ClearTargetMonster();
            controller.stateMachine.ChangeState(controller.RunHumanState); // 도망 상태로 전환
            StageManager.Instance.ChangeGold(_coin);    // 스테이지 보유 재화 갱신
            ReturnHumanToPool(3.0f); // 인간을 풀로 반환
        }
    }
    
    // 지연시간 이후에 인간을 풀로 반환
    public void ReturnHumanToPool(float delay)
    {
        if (isReturning) return;   // 풀로 반환하는 중이면 실행 x
        
        isReturning = true;
        
        if (gameObject.activeSelf)   // Scene에 활성화 상태일 때만 실행
            StartCoroutine(ReturnHumanProcess(delay));
    }
    
    // 지연시간 이후에 인간을 풀로 반환하는 코루틴
    private IEnumerator ReturnHumanProcess(float delay)
    {
        yield return new WaitForSeconds(delay);
        HumanManager.Instance.SubHumanCount(SpawnedWaveIdx);    // 스폰된 웨이브에서 인간 카운트 횟수 차감
        GameManager.Instance.RemoveActiveList(this);
        PoolManager.Instance.ReturnToPool(gameObject.name, this);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReturning) return;
        
        // 도망가는 인간과 다른 상태의 인간이 서로 밀리지 않도록 회피 타입 변경
        if (other.CompareTag("Human"))  // 인간이 트리거되면
        {
            Human human = other.gameObject.GetComponent<Human>();
            if (human.isReturning)  // 반환 중인 상태면
            {
                // 자신의 회피 타입을 None으로 설정, 서로 충돌하지 않고 지나가게 만듦
                controller.Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }
        }

        // 가장 가까이에 있는 몬스터 공격하도록 설정
        if (other.CompareTag("Monster"))
        {
            if (controller.TargetMonster == null)   // 타겟 몬스터가 없으면
            {
                controller.SetTargetMonster(other.gameObject.transform);    // 트리거된 몬스터를 타겟 몬스터로 설정
                return;
            }
            
            // 타겟 몬스터가 있는 상태이면
            float newDistance = (other.transform.position - transform.position).magnitude;
            float distance = (controller.TargetMonster.position - transform.position).magnitude;    
            if (newDistance < distance) // 새로 트리거된 몬스터가 기존 몬스터보다 가까우면
            {
                controller.SetTargetMonster(other.gameObject.transform);    // 타겟 몬스터 변경
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 몬스터 콜라이더 범위 벗어나면 타겟 몬스터 해제
        if (other.CompareTag("Monster"))
        {
            controller.ClearTargetMonster();
        }
    }
}