using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    [SerializeField] private HumanSO humanData;
    private int _id;
    private float _fearLevel;
    private float _maxFear;
    private int _coin;  // 놀랐을 때 떨어뜨리고 가는 재화량(처치 시 획득 재화량)
    public int LifeInflicted { get; private set; }
    public int SpawnedWaveIdx { get; set; }
    private bool _isReturning;  // 풀에 반환중인 상태인지 체크

    public HumanController controller;
    [SerializeField] private Image fearGauge;

    private void Awake()
    {
        humanData = HumanDataLoader.Instance.GetHumanByName(gameObject);
        _maxFear = humanData.maxFear;
        _coin = humanData.coin;
        LifeInflicted = humanData.lifeInflicted;
        
        if (controller == null)
        {
            controller = gameObject.AddComponent<HumanController>();
        }

        if (fearGauge == null)
        {
            fearGauge = gameObject.transform.Find("Canvas/FearGauge/Front").GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        // 활성화 시 매번 공포 수치와 UI 초기화
        _fearLevel = 0;
        fearGauge.fillAmount = 0;
        _isReturning = false;   // 반환하고 있지 않은 상태로 전환
        
        // 게임 종료 이벤트 발생하면 풀로 바로 반환
        HumanManager.Instance.OnGameClear -= () => { PoolManager.Instance.ReturnToPool(gameObject.name, gameObject); };
        HumanManager.Instance.OnGameClear += () => { PoolManager.Instance.ReturnToPool(gameObject.name, gameObject); };
        StageManager.Instance.OnGameOver -= () => { PoolManager.Instance.ReturnToPool(gameObject.name, gameObject); };
        StageManager.Instance.OnGameOver += () => { PoolManager.Instance.ReturnToPool(gameObject.name, gameObject); };
    }

    // 인간의 공포 수치를 올리는 메서드
    public void IncreaseFear(float amount)
    {
        // 놀라는 효과음과 애니메이션 실행
        SoundManager.Instance.PlaySFX(SfxType.SurprisingHuman);
        controller.animator.SetTrigger("Surprise");
        
        _fearLevel = Mathf.Min(_fearLevel + amount, _maxFear); // 최대값 넘지 않도록 제한
        fearGauge.fillAmount = _fearLevel / _maxFear;   // UI 갱신
        if (_fearLevel >= _maxFear) // 갱신된 값이 최대값보다 크면
        {
            controller.animator.SetBool("IsBattle", false);
            controller.StateMachine.ChangeState(controller.RunHumanState); // 도망 상태로 전환
            StageManager.Instance.ChangeGold(_coin);    // 스테이지 보유 재화 갱신
            ReturnHumanToPool(3.0f); // 인간을 풀로 반환
        }
    }
    
    // 지연시간 이후에 인간을 풀로 반환하는 메서드
    public void ReturnHumanToPool(float delay)
    {
        if (_isReturning) return;   // 풀로 반환하는 중이면 실행 x
        
        _isReturning = true;
        HumanManager.Instance.SubHumanCount(SpawnedWaveIdx);    // 스폰된 웨이브에서 인간 카운트 횟수 차감
        
        if (gameObject.activeInHierarchy)   // Scene에 활성화 상태일 때만 실행
            StartCoroutine(ReturnHumanProcess(delay));
    }
    
    // 지연시간 이후에 인간을 풀로 반환하는 코루틴
    private IEnumerator ReturnHumanProcess(float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.ReturnToPool(gameObject.name, gameObject);
    }
}