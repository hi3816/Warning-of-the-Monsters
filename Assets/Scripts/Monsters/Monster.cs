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
    
    private Animator _animator;
    private MonsterState _monsterState;
    
    private float _currentFatigue; //현재 피로도
    public float CurrentFatigue
    {
        get => _currentFatigue;
        private set => _currentFatigue = Mathf.Clamp(value, 0f, data.fatigue);
    }
    
    private float _lastScareTime;

    private void Awake()
    {
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
        // CurrentFatigue += (Time.deltaTime * Random.Range(human.minFatigueInflicted, human.maxFatigueInflicted));
        if (Time.time - _lastScareTime > data.cooldown)
        {
            InflictFear();
        }
    }
        
    protected virtual void InflictFear()
    {
        _lastScareTime = Time.time;
        // human.currentFear += (Time.deltaTime * data.fearInflicted);
        if (CurrentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
        }
    }

    private void ReturnToVillage()
    {
        //fading out, 1f
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(data.poolTag, gameObject);
    }
}