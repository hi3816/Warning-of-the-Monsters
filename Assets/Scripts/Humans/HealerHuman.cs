using System.Collections;
using UnityEngine;

public interface IHealer
{
    void HealFear(float amount);
}

public class HealerHuman : Human, IHealer
{
    private float _healAmount;
    private float _healRadius;
    private float _healCooldown;
    private Coroutine _healCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _healRadius = 2;
        _healAmount = base.controller.MaxFatigueInflicted * 0.8f;
        _healCooldown = base.controller.Cooldown * 0.5f;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (_healCoroutine == null)
        {
            _healCoroutine = StartCoroutine(HealingCoroutine());
        }
    }
    
    public void HealFear(float amount)
    {
        if (isReturning) return;
        
        Collider2D[] humansInRange = Physics2D.OverlapCircleAll(transform.position, _healRadius);
        foreach (Collider2D collider in humansInRange)
        {
            Human otherHuman = collider.GetComponent<Human>();
            if (otherHuman != null && !otherHuman.isReturning)
            {
                otherHuman.DecreaseFear(amount);
            }
        }
    }
    
    private IEnumerator HealingCoroutine()
    {
        while (true)
        {
            HealFear(_healAmount);
            yield return new WaitForSeconds(_healCooldown);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_healCoroutine != null)
        {
            StopCoroutine(_healCoroutine);
            _healCoroutine = null;
        }
    }
}