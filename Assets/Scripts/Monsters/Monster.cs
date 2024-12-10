using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Idle,
    Scaring, //scare human distance
    Summoning, //for summonerMonster
    Walking, //for minion
    ReturningVillage
}

public abstract class Monster : MonoBehaviour
{
    public MonsterSO data;
    private List<Human> _targetHumanList = new List<Human>();
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private MonsterState _monsterState;
    public float fadeDuration = 0.5f;
    public int currentUpgradeLevel = 0;
    private float _lastScareTime;
    public float currentFatigue; //현재 피로도
    private Coroutine coroutine;
    private Collider2D _collider;
    public MonsterUpgradeUI upgradeUI;
    
    public void Upgrade(Monster_Data.Upgrade_Data upgradeData)
    {
        currentUpgradeLevel = upgradeData.upgrade_level;
        data.fatigue = upgradeData.fatigue;
        data.fearInflicted = upgradeData.fearInflicted;
        data.cooldown = upgradeData.cooldown;
        data.requiredCoins = upgradeData.requiredCoins;
    }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        SetState(MonsterState.Idle);
    }
    
    private void OnMouseDown()
    {
        if (upgradeUI != null)
        {
            upgradeUI.Show(this);
        }
    }
    
    protected virtual void Update()
    {
        Transform nearestHuman = GetNearestHuman();
        
        switch (_monsterState)
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
    
    private Transform GetNearestHuman()
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

    private void SetState(MonsterState state)
    {
        if (_monsterState == state) return;
        
        _monsterState = state;
        
        switch (_monsterState)
        {
            case MonsterState.Idle:
                _animator.SetBool("Scare", false);
                break;

            case MonsterState.Scaring:
                _animator.SetBool("Scare", true);
                break;

            case MonsterState.ReturningVillage:
                _animator.SetTrigger("Return");
                coroutine = StartCoroutine(FadeOutAndReturnToPool());
                break;
        }
    }
    
    private void UpdateAnimatorParameters(Vector2 direction)
    {
        if (_animator == null) return;

        _animator.SetFloat("Horizontal", direction.x);
        _animator.SetFloat("Vertical", direction.y);
        
        bool isScaring = direction != Vector2.zero;
        _animator.SetBool("Scare", isScaring);
    }
    
    private void Scaring()
    {
        // 단일 공격
        foreach (Human human in _targetHumanList)
        {
            if (human == null) continue;
        
            if (Time.time - _lastScareTime > data.cooldown)
            {
                human.IncreaseFear(data.fearInflicted);
                //human.controller.SetTargetMonster(transform);
                _lastScareTime = Time.time;
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
        if (_monsterState == MonsterState.ReturningVillage) return;
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            if (human != null && !_targetHumanList.Contains(human))
            {
                _targetHumanList.Add(human);
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