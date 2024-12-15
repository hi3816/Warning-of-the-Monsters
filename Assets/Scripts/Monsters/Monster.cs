using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int minionId;
    public float monsterId;
    public int currentLevel;
    public string poolTag;
    public float fatigue; //몬스터 피로도바 최대치
    public float fearInflicted; //적(인간)에게 주는 공포수치량
    public float cooldown; //몬스터 놀래킴 쿨타임
    public float humanScaringRange; //적(인간)을 놀래킬 수 있는 범위
    public float speed; //미니언 걷는 속도
    public int requiredCoins; //필요재화
    public int maxLevel; // 최대 레벨 -> 진화
}

public abstract class Monster : MonoBehaviour
{
    public MonsterData data;
    protected List<Human> _targetHumanList = new List<Human>();
    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected MonsterState MonsterState;
    private float fadeDuration = 0.5f;
    protected float LastScareTime;
    private float currentFatigue; //현재 피로도
    private Coroutine coroutine;
    
    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        SetState(MonsterState.Idle);
    }
    
    protected virtual void Update()
    {
        Transform nearestHuman = GetNearestHuman();
        
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
                ReturnToVillage();
                break;
        }
    }

    public void Upgrade(Monster_Data.Upgrade_Data upgradeData)
    {
        data.monsterId = upgradeData.monster_id;
        data.currentLevel = upgradeData.upgrade_level;
        data.fatigue = upgradeData.fatigue;
        data.fearInflicted = upgradeData.fearInflicted;
        data.cooldown = upgradeData.cooldown;
        data.humanScaringRange = upgradeData.humanScaringRange;
        data.requiredCoins = upgradeData.requiredCoins;
    }

    public void Evolution(Monster_Data.Evolution_Data evolutionData)
    {
        data.monsterId = evolutionData.evolution_id;
        data.currentLevel = evolutionData.upgrade_level;
        data.poolTag = evolutionData.name;
        data.fatigue = evolutionData.fatigue;
        data.fearInflicted = evolutionData.fearInflicted;
        data.cooldown = evolutionData.cooldown;
        data.humanScaringRange = evolutionData.humanScaringRange;
        data.requiredCoins = evolutionData.requiredCoins;
    }

    protected Transform GetNearestHuman()
    {
        Collider2D[] detectedHumans = Physics2D.OverlapCircleAll(transform.position, data.humanScaringRange, LayerMask.GetMask("Human"));
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
                Animator.SetTrigger("Return");
                coroutine = StartCoroutine(FadeOutAndReturnToPool());
                break;
        }
    }
    
    private void UpdateAnimatorParameters(Vector2 direction)
    {
        if (Animator == null) return;

        Animator.SetFloat("Horizontal", direction.x);
        Animator.SetFloat("Vertical", direction.y);
        
        bool isScaring = direction != Vector2.zero;
        Animator.SetBool("Scare", isScaring);
    }
    
    protected virtual void Scaring()
    {
        // 단일 공격
        foreach (Human human in _targetHumanList)
        {
            if (human == null) continue;
        
            if (Time.time - LastScareTime > data.cooldown)
            {
                human.IncreaseFear(data.fearInflicted);
                //human.controller.SetTargetMonster(transform);
                LastScareTime = Time.time;
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
        
        if (_targetHumanList.Count == 0)
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
            if (human != null && !_targetHumanList.Contains(human))
            {
                _targetHumanList.Add(human);
                if (!human.isReturning)
                    human.controller.SetTargetMonster(transform);
                SetState(MonsterState.Scaring);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            human.controller.ClearTargetMonster();
            if (human != null && _targetHumanList.Contains(human))
            {
                _targetHumanList.Remove(human);
            }
        }
    }
    
    public void IncreaseFatigue(float value)
    {
        currentFatigue += value;
        Animator.SetTrigger("Hit");
        Debug.Log($"Monster curFatigue: {currentFatigue}");
        if (currentFatigue >= data.fatigue)
        {
            currentFatigue = data.fatigue;
            SetState(MonsterState.ReturningVillage);
        }
    }
    
    private void ReturnToVillage()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
        foreach (var human in _targetHumanList)
        {
            if (human != null)
            {
                human.controller.ClearTargetMonster();
            }
        }
        
        _targetHumanList.Clear();
        
        // Fade out
        float startAlpha = _spriteRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
            yield return null;
        }

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0f);
        
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(data.poolTag, gameObject);
    }
    
    public void Reset()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        currentFatigue = 0f;
        _targetHumanList.Clear();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        SetState(MonsterState.Idle);
    }
    
    // public void ResetToBaseState()
    // {
    //     currentUpgradeLevel = 0;
    //     data = MonsterDataManager.Instance.GetBaseMonsterData(data.id);
    //     transform.localScale = Vector3.one;
    // }
}