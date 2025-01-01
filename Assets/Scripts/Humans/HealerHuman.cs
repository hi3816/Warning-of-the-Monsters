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

    [SerializeField] private Transform _healTransform;
    [SerializeField] private SpriteRenderer _healRenderer;
    
    private Coroutine _healCoroutine;
    private Coroutine _colorCoroutine;

    protected override void Awake()
    {
        base.Awake();
        
        if (_healTransform == null)
            _healTransform = gameObject.transform.Find("Canvas/MainSprite/HealRange").GetComponent<Transform>();
        if (_healRenderer == null)
            _healRenderer = gameObject.transform.Find("Canvas/MainSprite/HealRange").GetComponent<SpriteRenderer>();

        _healRadius = 0.5f;
        _healAmount = controller.MaxFatigueInflicted * 0.8f;
        _healCooldown = controller.Cooldown * 0.5f;
        _healTransform.localScale = new Vector3(_healRadius * 2, _healRadius * 2, 0);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (_healCoroutine == null)
        {
            _healCoroutine = StartCoroutine(HealingCoroutine());
        }
        _healRenderer.color = Color.clear;  // 시작 시 투명으로 초기화
    }
    
    public void HealFear(float amount)
    {
        if (isReturning || controller.TargetMonster == null) return;
        
        ShowHealRange();

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
    
    private void OnDrawGizmos() // 범위 확인용 (빌드 시 삭제하기)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _healRadius);
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
    
    private IEnumerator ColoringCoroutine()
    {
        float duration = 0.5f;
        Color startColor = new Color(0, 1, 0, 0);
        Color endColor = new Color(0, 1, 0, 0.5f);
    
        // duration 동안 반투명 녹색으로 만들기
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _healRenderer.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        // duration 동안 반투명으로 만들기
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _healRenderer.color = Color.Lerp(endColor, startColor, elapsed / duration);
            yield return null;
        }

        _healRenderer.color = startColor; // 처음 색상으로 초기화
    }

    private void ShowHealRange()
    {
        if (_colorCoroutine != null)
        {
            StopCoroutine(_colorCoroutine);
        }
        _colorCoroutine = StartCoroutine(ColoringCoroutine());
    }
}