using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    private HumanSO _humanData;
    private float _fearLevel;
    private float _maxFear;
    private int _coin;  // 놀랐을 때 떨어뜨리고 가는 재화량(처치 시 획득 재화량)
    public int LifeInflicted { get; private set; }
    public int WaveIdx { get; set; }

    public HumanController controller;
    [SerializeField] private Image fearGauge;

    private void Awake()
    {
        _humanData = CustomUtil.ResourceLoad<HumanSO>("SO/Human/HumanSO_0");
        _maxFear = _humanData.maxFear;
        _coin = _humanData.coin;
        LifeInflicted = _humanData.lifeInflicted;
        
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
    }

    // 인간의 공포 수치를 올리는 메서드
    public void IncreaseFear(float amount)
    {
        // 놀라는 효과음과 애니메이션 실행
        SoundManager.Instance.PlaySFX(SfxType.SurprisingHuman);
        controller.animator.SetTrigger("Surprise");
        controller.animator.SetBool("IsRun", true);
        
        _fearLevel = Mathf.Min(_fearLevel + amount, _maxFear); // 최대값 넘지 않도록 제한
        fearGauge.fillAmount = _fearLevel / _maxFear;   // UI 갱신
        // Debug.Log($"Fear: {_fearLevel}");
        if (_fearLevel >= _maxFear) // 갱신된 값이 최대값보다 크면
        {
            controller.ReturnHumanToPool(3.0f); // 인간을 풀로 반환
            controller.StateMachine.ChangeState(controller.RunHumanState); // 도망 상태로 전환
            StageManager.Instance.ChangeGold(_coin);    // 스테이지 보유 재화 갱신
        }
    }
}