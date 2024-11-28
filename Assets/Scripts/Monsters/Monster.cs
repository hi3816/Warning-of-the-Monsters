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
    private StageManager stageManager;
    public Transform monsterSpawnPoint;
    private Animator animator;
    private MonsterState monsterState;
    public string poolTag;
    private float currentFatigue = 0f; //현재 피로도
    private float lastScareTime;
    private bool isInBattle = false;

    private void Awake()
    {
        stageManager = FindObjectOfType<StageManager>();
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        switch (monsterState)
        {
            case MonsterState.Idle:
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

    protected virtual void SetState(MonsterState state)
    {
        monsterState = state;
        
        switch (state)
        {
            case MonsterState.Idle:
            case MonsterState.Detecting:
                break;
            case MonsterState.Scaring:
                animator.SetBool("Scare", true);
                isInBattle = true;
                break;
            case MonsterState.ReturningVillage:
                animator.SetBool("Scare", false);
                animator.SetTrigger("Return");
                break;
        }
    }

    // protected virtual void Place()
    // {
    //      if (stageManager.currGold >= data.requiredCoins) //if player has enough coins to place monster
    //      {
    //          stageManager.currGold -= data.requiredCoins;
    //          SpawnMonster();
    //          SetState(MonsterState.Detecting);
    //      }
    //      else
    //      {
    //          Debug.Log("not enough coins to place the monster");
    //      }
    // }
    
    private void SpawnMonster()
    {
        if (PoolManager.Instance != null)
        {
            GameObject monster = PoolManager.Instance.SpawnFromPool(poolTag, monsterSpawnPoint.position, Quaternion.identity);
            SetState(MonsterState.Detecting);
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
        // currentFatigue += (Time.deltaTime * //인간이 주는 피로도 변수 이름 들어갈 공간);
        
        if (Time.time - lastScareTime > data.cooldown)
        {
            InflictFear();
        }
        
        if (currentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
        }
    }
        
    protected virtual void InflictFear()
    {
        lastScareTime = Time.time;
        // human fear value += (Time.deltaTime * data.fearInflicted);
        // animator.SetTrigger("Scared");
    }

    private void ReturnToVillage()
    {
        //fading out, 1f
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(poolTag, gameObject);
    }
}