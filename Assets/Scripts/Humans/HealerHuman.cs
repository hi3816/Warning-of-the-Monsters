using System.Collections;
using UnityEngine;

public interface IHealer
{
    void HealFear(float amount);
}

public class HealerHuman : Human, IHealer
{
    // 힐 범위 시각적 표현
    [SerializeField] private Transform _healTransform;
    [SerializeField] private SpriteRenderer _healRenderer;
    private Coroutine _healCoroutine;   // 힐 실행하는 코루틴
    private Coroutine _colorCoroutine;  // 힐 범위 색상 표현하는 코루틴
    private float _healAmount;
    private float _healRadius;
    private float _healCooldown;

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
            _healCoroutine = StartCoroutine(HealingCoroutine());    // 활성화하며 힐 코루틴 시작
        }
        _healRenderer.color = Color.clear;  // 시작 시 투명으로 초기화
    }
    
    public void HealFear(float amount)
    {
        // 도망가고 있거나 비전투 중에는 실행 X
        if (isReturning || controller.TargetMonster == null) return;
        
        ShowHealRange();    // 힐 범위 화면에 출력

        // 힐 범위 내 반환하고 있는 상태가 아닌 인간들의 공포 수치 낮춤 (본인 포함)
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
        while (true)    // 쿨타임 동안 기다렸다가 힐 반복 실행
        {
            HealFear(_healAmount);
            yield return new WaitForSeconds(_healCooldown);
        }
    }

    private void OnDisable()
    {
        if (_healCoroutine != null)
        {
            StopCoroutine(_healCoroutine);
            _healCoroutine = null;
        }
        
        if (_colorCoroutine != null)
        {
            StopCoroutine(_colorCoroutine);
            _colorCoroutine = null;
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