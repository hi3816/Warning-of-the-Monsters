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
    protected bool isSingleTargetAttack = true;
    protected Projectile_Data projectileData;

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
                    isSingleTargetAttack = false;
                    this.projectileData = projectileData;
                }
            }
        }
        if (isSingleTargetAttack) projectileData = null;

        if (_fatigueGauge == null)
        {
            _fatigueGauge = gameObject.transform.Find("FatigueCanvas/FatigueGauge").gameObject;
        }
        _monsterFatigueGauge = GetComponent<MonsterFatigueGague>();
    }
    
    protected virtual void OnEnable()
    {
        GameManager.Instance.AddActiveList(this);
        ResetMonster();
        _fatigueGauge.SetActive(true);
    }
    
    protected virtual void Update()
    {
        Transform nearestHuman = GetNearestHuman();
        _lastScareTime += Time.deltaTime;
        switch (MonsterState)
        {
            case MonsterState.Idle:
                UpdateAnimatorParameters(Vector2.zero);
                break;
            case MonsterState.Scaring:
                if (nearestHuman != null)
                {
                    Vector2 directionToHuman = ((Vector2)nearestHuman.position - (Vector2)transform.position).normalized;
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

    protected void ResetMonster()
    {
        SetState(MonsterState.Idle);
        if (_spriteRenderer.color.a <= 1f)
        {
            Color color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;
        }
        _lastScareTime = data.cooldown;
    }

    // 처음 데이터 저장
    private void SetMonsterData(Monster_Data monsterData)
    {
        data.id = monsterData.id;
        data.currentLevel = 0;
        data.poolTag = monsterData.name;
        data.currentFatigue = 0f;
        data.fatigue = monsterData.fatigue;
        data.minFearInflicted = monsterData.minFearInflicted;
        data.maxFearInflicted = monsterData.maxFearInflicted;
        data.cooldown = monsterData.cooldown;
        data.humanDetectRange = monsterData.humanDetectRange;
        data.humanScaringRange = monsterData.humanScaringRange;
        data.requiredCoins = monsterData.requiredCoins;
        data.maxLevel = monsterData.maxLevel;
        data.walkSpeed = monsterData.walkSpeed;
        data.monsterType = monsterData.monsterType;
    }

    // 현재 데이터 변경
    public void SetMonsterDataToMonsterData(MonsterData newMonsterData)
    {
        data = newMonsterData.Clone();
    }

    public void Upgrade(Upgrade_Data upgradeData)
    {
        data.monsterId = upgradeData.monsterId;
        data.currentLevel = upgradeData.upgradeLevel;
        data.fatigue = upgradeData.fatigue;
        data.minFearInflicted = upgradeData.minFearInflicted;
        data.maxFearInflicted = upgradeData.maxFearInflicted;
        data.cooldown = upgradeData.cooldown;
        data.humanDetectRange = upgradeData.humanDetectRange;
        data.humanScaringRange = upgradeData.humanScaringRange;
        data.requiredCoins = upgradeData.requiredCoins;
        if (_monsterFatigueGauge != null) _monsterFatigueGauge.SetFatigue();
    }

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
        if(data.monsterType != MonsterType.Minion) data.humanDetectRange = data.humanScaringRange;
        data.requiredCoins = evolutionData.requiredCoins;
        SetState(MonsterState.Idle);
        if(_monsterFatigueGauge != null) _monsterFatigueGauge.SetFatigue();
    }

    protected Transform GetNearestHuman()
    {
        Collider2D[] detectedHumans = Physics2D.OverlapCircleAll(transform.position, data.humanDetectRange, LayerMask.GetMask("Human"));
        if (detectedHumans.Length > 0)
        {
            Transform nearestHuman = detectedHumans[0].transform;
            float minDistance = Vector2.Distance(transform.position, nearestHuman.position);

            foreach (Collider2D human in detectedHumans)
            {
                float distance = Vector2.Distance(transform.position, human.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestHuman = human.transform;
                }
            }
            return nearestHuman;
        }
        return null;
    }

    protected virtual void SetState(MonsterState state)
    {
        if (MonsterState == state) return;
        
        MonsterState = state;
        switch (MonsterState)
        {
            case MonsterState.Idle:
                Animator.SetBool("Scare", false);
                break;

            case MonsterState.Scaring:
                Animator.SetBool("Scare", true);
                break;

            case MonsterState.ReturningVillage:
                Animator.SetBool("Return", true);
                _coroutine = StartCoroutine(FadeOutAndReturnToPool());
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
        // 단일 공격
        foreach (Human human in TargetHumanList)
        {
            if (human == null) continue;
            
            if (_lastScareTime >= data.cooldown)
            {
                human.IncreaseFear(Random.Range(data.minFearInflicted, data.maxFearInflicted));
                _lastScareTime = 0f;
            }
        }

        // 범위 공격
        // if (Time.time - _lastScareTime > data.cooldown)
        // {
        //     foreach (Human human in _targetHumanList)
        //     {
        //         if (human == null) continue;
        //
        //         human.IncreaseFear(data.fearInflicted);
        //         //human.controller.SetTargetMonster(transform);
        //     }
        //     _lastScareTime = Time.time;
        // }
        
        if (TargetHumanList.Count == 0)
        {
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
                SetState(MonsterState.Scaring);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            if (human != null && TargetHumanList.Contains(human))
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

    public void SetFatigue(float value)
    {
        data.currentFatigue = value;
    }

    public virtual void ReturnToVillage()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
        foreach (var human in TargetHumanList)
        {
            if (human != null)
            {
                human.controller.ClearTargetMonster();
            }
        }
        TargetHumanList.Clear();
        
        _fatigueGauge.SetActive(false);
        
        // Fade out
        float startAlpha = _spriteRenderer.color.a;
        float elapsedTime = 0f;
        Color startColor = _spriteRenderer.color;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / _fadeDuration);
            startColor.a = newAlpha;
            _spriteRenderer.color = startColor;
            yield return null;
        }

        startColor.a = 0f;
        _spriteRenderer.color = startColor;
        PoolManager.Instance.ReturnToPool(data.poolTag, this);
    }
    
    public void Reset()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        data.currentFatigue = 0f;
        TargetHumanList.Clear();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        SetState(MonsterState.Idle);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveActiveList(this);
    }
}