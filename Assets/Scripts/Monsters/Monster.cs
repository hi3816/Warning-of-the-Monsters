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
    private HumanSO _humanData;
    private Human _human;
    private List<HumanController> _targethumanList = new List<HumanController>();
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private MonsterState _monsterState;
    public float fadeDuration = 1f;
    private float _lastScareTime;
    private float _currentFatigue; //현재 피로도
    private float CurrentFatigue
    {
        get => _currentFatigue;
        set => _currentFatigue = Mathf.Clamp(value, 0f, data.fatigue);
    }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    
    //void Update()
    protected virtual void Update()
    {
        Transform nearestHuman = GetNearestHuman();
        
        switch (_monsterState)
        {
            case MonsterState.Idle:
                UpdateAnimatorParameters(Vector2.zero);
                break;
            case MonsterState.Scaring:
                // Vector2 directionToHuman = (nearestHuman.position - transform.position).normalized;
                // UpdateAnimatorParameters(directionToHuman);
                Battle();
                break;
            case MonsterState.ReturningVillage:
                ReturnToVillage();
                break;
        }
    }
    
    private Transform GetNearestHuman()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector2 boxCenter = transform.position;
        Vector2 boxSize = boxCollider.size;
        float boxAngle = 0f;

        Collider2D[] detectedHumans = Physics2D.OverlapBoxAll(boxCenter, boxSize, boxAngle, LayerMask.GetMask("Human"));
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

    public void SetState(MonsterState state)
    {
        if (_monsterState == state) return;
        
        _monsterState = state;
        
        switch (state)
        {
            case MonsterState.Scaring:
                _animator.SetBool("Scare", true);
                break;
            case MonsterState.ReturningVillage:
                _animator.SetTrigger("Return");
                break;
        }
    }
    
     private void UpdateAnimatorParameters(Vector2 direction)
     {
         if (_animator == null) return;

         _animator.SetFloat("Horizontal", direction.x);
         _animator.SetFloat("Vertical", direction.y);
     }
     
    protected virtual void Battle()
    {
        // CurrentFatigue += Time.deltaTime * Random.Range(_humanData.minFatigueInflicted, _humanData.maxFatigueInflicted);
        // if (Time.time - _lastScareTime > data.cooldown)
        // {
        //     InflictFear();
        // }
        if (_targethumanList.Count <= 0)
            SetState(MonsterState.Idle);
        foreach (HumanController human in _targethumanList)
        {
            if (Time.time - _lastScareTime > data.cooldown)
            {
                _lastScareTime = Time.time;
                human.IncreaseFear(data.fearInflicted);
                human.SetTargetMonster(this);
            }
        }
    }
        
    protected virtual void InflictFear()
    {
        _lastScareTime = Time.time;
        _human.FearLevel += (Time.deltaTime * data.fearInflicted);
        if (CurrentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Human"))
        {
            _targethumanList.Add(other.gameObject.GetComponent<HumanController>());
            SetState(MonsterState.Scaring);
        }
    
        if (CurrentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
        }
    }
    public void IncreaseFatigue(float value)
    {
        Debug.Log($"Monster curFatigue is {_currentFatigue}");
        
        _currentFatigue += value;
        if (_currentFatigue >= data.fatigue)
        {
            _currentFatigue = data.fatigue;
            Debug.LogAssertion($"Monster returns to Village");
            SetState(MonsterState.ReturningVillage);
        }
    }
    
    private void ReturnToVillage()
    {
        StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
        foreach (var human in _targethumanList)
        {
            if (human != null)
            {
                human.SetTargetMonster(null);
            }
        }
        _targethumanList.Clear();
        yield return StartCoroutine(FadeOut());
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(data.poolTag, gameObject);
    }
    
    private IEnumerator FadeOut()
    {
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
    }
}