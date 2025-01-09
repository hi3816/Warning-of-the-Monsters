using UnityEngine;

public class HumanParticle : SingletonBase<HumanParticle>
{
    public Transform humanEffect;
    public ParticleSystem attackParticle;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        if (humanEffect == null)
            humanEffect = transform.Find("Effects").transform;
        if (attackParticle == null)
            attackParticle = humanEffect.gameObject.GetComponentInChildren<ParticleSystem>();
    }
}
