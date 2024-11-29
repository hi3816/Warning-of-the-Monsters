using Unity.Properties;
using UnityEngine;

public enum MonsterState
{
    Idle,
    Detecting, //detect human distance
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
    private bool isInBattle = false;

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
            case MonsterState.Detecting:
                // if (HumanInRange())
                // {
                //     SetState(MonsterState.Scaring);
                // }
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
                isInBattle = true;
                break;
            case MonsterState.ReturningVillage:
                _animator.SetTrigger("Return");
                break;
        }
    }
    
    // protected virtual void DetectionRange()
    // {
    //     //monster detect human when human walk into the humanDetectionRange
    //     if (HumanInRange())
    //     {
    //         SetState(MonsterState.Scaring);
    //     }
    // }

    // private bool HumanInRange()
    // {
    //     // Collider2D[] detectedHumans = Physics2D.OverlapCircleAll(transform.position, data.humanDetectionRange);
    //     // foreach (var human in detectedHumans)
    //     // {
    //     //     if (human.CompareTag("Human")) //let's see
    //     //     {
    //     //         return true;
    //     //     }
    //     // }
    //     // return false;
    // }

    protected virtual void Battle()
    {
        // CurrentFatigue += (Time.deltaTime * Random.Range(human.minFatigueInflicted, human.maxFatigueInflicted));
        if (Time.time - _lastScareTime > data.cooldown)
        {
            InflictFear();
        }
        
        if (CurrentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
        }
    }
        
    protected virtual void InflictFear()
    {
        _lastScareTime = Time.time;
        // human.currentFear += (Time.deltaTime * data.fearInflicted);
        // animator.SetTrigger("Scared");
    }

    private void ReturnToVillage()
    {
        //fading out, 1f
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(data.poolTag, gameObject);
    }
}