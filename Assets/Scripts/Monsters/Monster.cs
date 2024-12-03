using System.Collections;
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
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private MonsterState _monsterState;
    public float fadeDuration = 1f;
    private float _lastScareTime;
    private float _currentFatigue; //현재 피로도
    public float CurrentFatigue
    {
        get => _currentFatigue;
        private set => _currentFatigue = Mathf.Clamp(value, 0f, data.fatigue);
    }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        switch (_monsterState)
        {
            case MonsterState.Idle:
                _animator.SetBool("Idle", true);
                break;
            case MonsterState.Scaring:
                Battle();
                break;
            case MonsterState.ReturningVillage:
                ReturnToVillage();
                break;
        }
    }

    public void SetState(MonsterState state)
    {
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

     private void HumanInRange()
     {
         Collider2D[] detectedHumans = Physics2D.OverlapCircleAll(transform.position, data.humanScaringRange);
         if (detectedHumans.Length > 0)
         {
             SetState(MonsterState.Scaring);
         }
     }

    protected virtual void Battle()
    {
        CurrentFatigue += Time.deltaTime * Random.Range(_humanData.minFatigueInflicted, _humanData.maxFatigueInflicted);
        if (Time.time - _lastScareTime > data.cooldown)
        {
            InflictFear();
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
        Debug.LogAssertion("Human entered");
        _lastScareTime = Time.time;
    
        HumanController humanController = other.GetComponent<HumanController>();
        if (humanController != null)
        {
            humanController.human.targetMonster = this;
        }
    
        if (CurrentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
        }
    }
    
    private void ReturnToVillage()
    {
        StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
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