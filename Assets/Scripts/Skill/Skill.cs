using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public DataTable.Skill_Data SkillData { get; private set; }
    private RectTransform _rectTransform;
    private Animator _animator;
    private WaitForSeconds _animationTime;
    private Coroutine _attackCoroutine;
    private Coroutine _debuffCoroutine;
    private WaitForSeconds _effectDurationTime;
    private CircleCollider2D _skillCollider;
    private SpriteRenderer _spriteRenderer;
    [field: SerializeField] public int SkillIdx { get; private set; }

    // human 확인 리스트
    private List<GameObject> _humanList = new List<GameObject>();
    private float _agentOriginSpeed = 0f;

    private void Awake()
    {
        SkillData = DataManager5.Instance.GetSkillByIndex(SkillIdx);
        SetComponent();
    }

    // 컴포넌트 캐싱
    private void SetComponent()
    {
        // 애니메이션 길이 저장
        _animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _animationTime = new WaitForSeconds(stateInfo.length);

        transform.localScale = Vector3.one * SkillData.range;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _skillCollider = GetComponent<CircleCollider2D>();
        _effectDurationTime = new WaitForSeconds(SkillData.duration);
    }

    public void StartSkill()
    {
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        _attackCoroutine = StartCoroutine(CoEndSkillEffect());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            // 이미 스킬에 맞았으면 계산하지 않기
            if (_humanList.Contains(collision.gameObject)) return;

            // 리스트에 없으면 추가하고 스킬 공격력 만큼 피해 입히기
            _humanList.Add(collision.gameObject);
            CheckSkillType(collision.gameObject);
        }
    }

    private void CheckSkillType(GameObject humanObj)
    {
        switch (SkillData.skillType)
        {
            case SkillType.Attack:
                StartSkill();
                humanObj.GetComponent<Human>().IncreaseFear(SkillData.power);
                break;
            
            case SkillType.Debuff:
                StartSkill();
                HumanController humanController = humanObj.GetComponent<HumanController>();
                CheckSkillName(humanObj, humanController);
                if (_debuffCoroutine != null) StopCoroutine(_debuffCoroutine);
                StartCoroutine(CoEndDebuffSkill());
                break;
        }
    }

    private void CheckSkillName(GameObject humanObj, HumanController humanController)
    {
        if (SkillData.skillName == SkillName.FrozenGround)
        {
            _agentOriginSpeed = humanController.Agent.speed;
            humanObj.GetComponent<Human>().IncreaseFear(SkillData.power);
            humanController.Agent.speed *= ((100 - SkillData.percentage) / 100f);
        }
    }

    // 스킬이 끝났을 때
    private IEnumerator CoEndSkillEffect()
    {
        yield return _animationTime;

        if (SkillData.skillType == SkillType.Attack)
        {
            PoolManager.Instance.ReturnToPool(SkillData.skillName.ToString(), gameObject);
            _humanList.Clear();
        }
        else
        {
            _skillCollider.enabled = false;
            _spriteRenderer.enabled = false;
            if(_humanList.Count == 0)
            {
                if(_debuffCoroutine != null) StopCoroutine(_debuffCoroutine);
                StartCoroutine(CoEndDebuffSkill());
            }
        }
    }

    private IEnumerator CoEndDebuffSkill()
    {
        yield return _effectDurationTime;

        foreach (GameObject obj in _humanList)
        {
            UnityEngine.AI.NavMeshAgent agent = obj.GetComponent<HumanController>().Agent;
            agent.speed = _agentOriginSpeed;
        }

        _spriteRenderer.enabled = true;
        _skillCollider.enabled = true;
        _humanList.Clear();
        PoolManager.Instance.ReturnToPool(SkillData.skillName.ToString(), gameObject);
    }
}
