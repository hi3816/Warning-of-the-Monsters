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
    public Transform monsterSpawnPoint;
    private Animator animator;
    private MonsterState monsterState;
    public string monsterTag = "Monster";
    public string poolTag;
    private float currentFatigue = 0f; //현재 피로도
    private float lastSpawnTime;
    private float lastScareTime;
    private bool isInBattle = false;

    private void Awake()
    {
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
            case MonsterState.Scaring:
                animator.SetBool("Scare", true);
                isInBattle = true;
                break;

            case MonsterState.ReturningVillage:
                //animator.SetTrigger("Return"); or whatever
                break;
            
            case MonsterState.Idle:
                animator.SetBool("Scare", false);
                animator.SetBool("Idle", true);
                break;
            case MonsterState.Detecting:
                animator.SetBool("Scare", false);
                break;
        }
    }

    protected virtual void Place()
    {
        //monster need coin for place on the map
        // if (playerCoins >= data.requiredCoins && (Time.time - lastSpawnTime > spawnDelay)) //if player has enough coins to place monster
        // {
        //     playercoins -= data.requiredCoins;
        //     SpawnMonster();
        //     lastSpawnTime = Time.time;
        //     monsterState = MonsterState.Detecting;
        // }
        // else
        // {
        //     Debug.Log("not enough coins to place the monster");
        // }
    }
    
    private void SpawnMonster()
    {
        if (PoolManager.Instance != null)
        {
            GameObject monster = PoolManager.Instance.SpawnFromPool(monsterTag, monsterSpawnPoint.position, Quaternion.identity);
        }
    }

    // protected virtual void DetectionRange()
    // {
    //     //monster detect human when human walk into the humanDetectionRange
    //     if (HumanInRange())
    //     {
    //         SetState(MonsterState.Detecting);
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
        
        if (data.cooldown <= 0)
        {
            InflictFear();
            lastScareTime = Time.time;
        }
        
        if (currentFatigue >= data.fatigue)
        {
            SetState(MonsterState.ReturningVillage);
            ReturnToVillage();
        }
    }
        
    protected virtual void InflictFear()
    {
        if (Time.time - lastScareTime > data.cooldown)
        {
            lastScareTime = Time.time;
            // human fear value += (Time.deltaTime * data.fearInflicted);
            // animator.SetTrigger("Scared");
        }
    }

    private void ReturnToVillage()
    {
        //fading out, 1f
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(monsterTag, gameObject);
    }
}