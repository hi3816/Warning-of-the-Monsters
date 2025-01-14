using DataTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MonsterState
{
    Idle,
    Scaring, //scare human distance
    Walking, //for minion
    ReturningVillage
}

[System.Serializable]
public class MonsterData
{
    public int id;
    public int monsterId;
    public int currentLevel;
    public string poolTag;
    public float currentFatigue; //현재 피로도
    public float fatigue; //몬스터 피로도바 최대치
    public float minFearInflicted;
    public float maxFearInflicted;
    public float cooldown; //몬스터 놀래킴 쿨타임
    public float humanDetectRange;
    public float humanScaringRange; //적(인간)을 놀래킬 수 있는 범위
    public float walkSpeed; //미니언 걷는 속도
    public int requiredCoins; //필요재화
    public int maxLevel; // 최대 레벨 -> 진화
    public MonsterType monsterType;
    public EvolutionType evolutionType;

    public MonsterData Clone()
    {
        return new MonsterData
        {
            id = this.id,
            monsterId = this.monsterId,
            currentLevel = this.currentLevel,
            poolTag = this.poolTag,
            currentFatigue = this.currentFatigue,
            fatigue = this.fatigue,
            minFearInflicted = this.minFearInflicted,
            maxFearInflicted= this.maxFearInflicted,
            cooldown = this.cooldown,
            humanDetectRange = this.humanDetectRange,
            humanScaringRange = this.humanScaringRange,
            walkSpeed = this.walkSpeed,
            requiredCoins = this.requiredCoins,
            maxLevel = this.maxLevel,
            monsterType = this.monsterType,
            evolutionType = this.evolutionType,
        };
    }
}

public abstract class Monster : MonoBehaviour
{
    public MonsterData data;
    protected List<Human> TargetHumanList = new List<Human>();
    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected MonsterState MonsterState;
    private float _fadeDuration = 1f;
    protected float _lastScareTime;
    private Coroutine _coroutine;
    [SerializeField] private GameObject _fatigueGauge;
    protected MonsterFatigueGague _monsterFatigueGauge;
    public Action OnAttacked;
    protected bool _isSingleTargetAttack = true;
    protected Projectile_Data _projectileData;
    private CircleCollider2D _rangeCircleCollider;
    private Collider2D[] _collider2Ds;
    public Action OnReturnToVillage;
    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        SetState(MonsterState.Idle);
        var baseMonsterData = DataManager.Instance.GetBaseMonsterByIdx(data.id - 1);
        if (baseMonsterData != null)
        {
            SetMonsterData(baseMonsterData);
        }

        List<Projectile_Data> list = DataManager.Instance.GetProjectileData();
        foreach (Projectile_Data projectileData in list)
        {
            foreach (int id in projectileData.monsterId)
            {
                if (id == data.id)
                {
                    _isSingleTargetAttack = false;
                    this._projectileData = projectileData;
                }
            }
        }
        
        if (_isSingleTargetAttack) _projectileData = null;

        if (_fatigueGauge == null)
        {
            _fatigueGauge = gameObject.transform.Find("FatigueCanvas/FatigueGauge").gameObject;
        }
        
        _monsterFatigueGauge = GetComponent<MonsterFatigueGague>();

        _collider2Ds = GetComponentsInChildren<Collider2D>();
        ChangeDetectRangeCollider();
    }
    
    protected virtual void OnEnable()
    {
        ResetMonster();
    }
    
    protected virtual void Update()
    {
        _lastScareTime += Time.deltaTime;
        switch (MonsterState)
        {
            case MonsterState.Idle:
                UpdateAnimatorParameters(Vector2.zero);
                if (TargetHumanList.Count > 0)
                {
                    SetState(MonsterState.Scaring);
                }
                break;
            case MonsterState.Scaring:
                if (TargetHumanList.Count> 0)
                {
                    Vector2 directionToHuman = ((Vector2)TargetHumanList[0].transform.position - (Vector2)transform.position).normalized;
                    UpdateAnimatorParameters(directionToHuman);
                    Scaring();
                }
                else
                {
                    SetState(MonsterState.Idle);
                }
                break;
            case MonsterState.ReturningVillage:
                UpdateAnimatorParameters(StageManager.Instance.EndPoint.position);
                ReturnToVillage();
                break;
        }
    }

    private void ResetMonster()
    {
        SetState(MonsterState.Idle);
        _lastScareTime = data.cooldown;
        data.currentFatigue = 0f;
        TargetHumanList.Clear();
        _fatigueGauge.SetActive(true);
        
        if (_spriteRenderer.color.a <= 1f)
        {
            Color color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;
        }

        foreach (Collider2D collider in _collider2Ds)
        {
            collider.enabled = true;
        }
    }

    // 처음 데이터 저장
    protected void SetMonsterData(Monster_Data monsterData)
    {
        data.id = monsterData.id;
        data.currentLevel = 0;
        data.poolTag = monsterData.name;
        data.currentFatigue = 0f;
        data.fatigue = monsterData.fatigue[data.currentLevel];
        data.minFearInflicted = monsterData.minFearInflicted[data.currentLevel];
        data.maxFearInflicted = monsterData.maxFearInflicted[data.currentLevel];
        data.cooldown = monsterData.cooldown[data.currentLevel];
        data.humanDetectRange = monsterData.humanDetectRange[data.currentLevel];
        data.humanScaringRange = monsterData.humanScaringRange[data.currentLevel];
        data.requiredCoins = monsterData.requiredCoins[data.currentLevel];
        data.maxLevel = monsterData.maxLevel;
        data.walkSpeed = monsterData.walkSpeed[data.currentLevel];
        data.monsterType = monsterData.monsterType;
    }

    public void Upgrade(Monster_Data monsterData)
    {
        int nextLevel = data.currentLevel + 1;
        data.currentLevel = nextLevel;
        data.monsterId = monsterData.id;
        data.fatigue = monsterData.fatigue[nextLevel];
        data.minFearInflicted = monsterData.minFearInflicted[nextLevel];
        data.maxFearInflicted = monsterData.maxFearInflicted[nextLevel];
        data.cooldown = monsterData.cooldown[nextLevel];
        data.humanDetectRange = monsterData.humanDetectRange[nextLevel];
        data.humanScaringRange = monsterData.humanScaringRange[nextLevel];
        data.requiredCoins = monsterData.requiredCoins[nextLevel];
        if (_monsterFatigueGauge != null) _monsterFatigueGauge.SetFatigue();
        ChangeDetectRangeCollider();
    }

    // 단순 데이터 변경용
    public void Evolution(Evolution_Data evolutionData)
    {
        data.currentFatigue = 0f;
        data.monsterId = evolutionData.evolutionId;
        data.currentLevel = evolutionData.upgradeLevel;
        data.evolutionType = evolutionData.evolutionType;
        data.monsterType = evolutionData.monsterType;
        data.poolTag = evolutionData.evolutionName;
        data.fatigue = evolutionData.fatigue;
        data.minFearInflicted = evolutionData.minFearInflicted;
        data.maxFearInflicted = evolutionData.maxFearInflicted;
        data.cooldown = evolutionData.cooldown;
        data.humanScaringRange = evolutionData.humanScaringRange;
        // 미니언 진화 X -> 소환사 및 일반 몬스터 humanScaringRange == humanDetectRange
        data.humanDetectRange = data.humanScaringRange;
        data.requiredCoins = evolutionData.requiredCoins;

        SetState(MonsterState.Idle);
        if (_monsterFatigueGauge != null) _monsterFatigueGauge.SetFatigue();
    }

    public void ChangeDetectRangeCollider()
    {
        if (_rangeCircleCollider == null) _rangeCircleCollider = GetComponent<CircleCollider2D>();
        _rangeCircleCollider.radius = data.humanDetectRange * 0.5f;
    }

    protected void SetState(MonsterState state)
    {
        if (MonsterState == state) return;
        
        MonsterState = state;
        switch (MonsterState)
        {
            case MonsterState.Idle:
                break;

            case MonsterState.Scaring:
                Animator.SetTrigger("Scare");
                break;

            case MonsterState.ReturningVillage:
                Animator.SetBool("Return", true);
                break;
        }
    }
    
    protected void UpdateAnimatorParameters(Vector2 direction)
    {
        if (Animator == null) return;
        
        Animator.SetFloat("Horizontal", direction.x);
        Animator.SetFloat("Vertical", direction.y);
    }

    protected virtual void Scaring()
    {
        if (_lastScareTime >= data.cooldown)
        {
            Human human = TargetHumanList[0];
            if (human == null) return;
            human.IncreaseFear(Random.Range(data.minFearInflicted, data.maxFearInflicted));

            _lastScareTime = 0f;
            SetState(MonsterState.Idle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MonsterState == MonsterState.ReturningVillage) return;
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            if (human != null && !TargetHumanList.Contains(human) && human.FearLevel < human.MaxFear)
            {
                TargetHumanList.Add(human);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            if (human != null)
            {
                TargetHumanList.Remove(human);
            }
        }
    }
    
    public void IncreaseFatigue(float value)
    {
        data.currentFatigue += value;
        Animator.SetTrigger("Hit");
        if (data.currentFatigue >= data.fatigue)
        {
            data.currentFatigue = data.fatigue;
            SetState(MonsterState.ReturningVillage);
        }
        OnAttacked?.Invoke();
    }

    public virtual void ReturnToVillage()
    {
        OnReturnToVillage?.Invoke();
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
        foreach (Collider2D collider in _collider2Ds)
        {
            collider.enabled = false;
        }

        foreach (var human in TargetHumanList)
        {
            if (human != null)
            {
                human.controller.ClearTargetMonster();
                human.controller.targetMonsterList.Remove(transform);
                human.controller.SetTargetMonster();
            }
        }

        TargetHumanList.Clear();
        _fatigueGauge.SetActive(false);

        //// Fade out
        //float startAlpha = _spriteRenderer.color.a;
        //float elapsedTime = 0f;
        //Color startColor = _spriteRenderer.color;

        //while (elapsedTime < _fadeDuration)
        //{
        //    elapsedTime += Time.deltaTime;
        //    float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / _fadeDuration);
        //    startColor.a = newAlpha;
        //    _spriteRenderer.color = startColor;
        //    yield return null;
        //}
        //startColor.a = 0f;
        //_spriteRenderer.color = startColor;

        if (this.GetType().Name != "Minion")
        {
            GameManager.Instance.RemoveActiveList(this);
            PoolManager.Instance.ReturnToPool(data.poolTag, this);
        }
        yield return null;
    }
}