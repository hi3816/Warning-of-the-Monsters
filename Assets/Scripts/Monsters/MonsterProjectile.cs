using DataTable;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterProjectile : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;
    private MonsterData _monsterData;
    private Projectile_Data _data;
    private Vector2 _target;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private WaitForSeconds _animationTime;
    private Coroutine _flightCoroutine;
    private Coroutine _destroyCoroutine;
    private List<GameObject> _humanList = new List<GameObject>();

    private void Awake()
    {
        _humanList.Clear();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _animationTime = new WaitForSeconds(stateInfo.length * 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            Human human = collision.GetComponent<Human>();
            
            if (_humanList.Contains(collision.gameObject) || human.FearLevel >= human.MaxFear) return;

            // 리스트에 없으면 추가하고 스킬 공격력 만큼 피해 입히기
            _humanList.Add(collision.gameObject);

            human.IncreaseFear(Random.Range(_monsterData.minFearInflicted, _monsterData.maxFearInflicted));
            DestroyProjectile();
        }
    }

    public void SetProjectileInfo(Vector2 target, MonsterData monsterData, Projectile_Data projectileData)
    {
        _monsterData = monsterData;
        _target = target;
        _data = projectileData;
        transform.localScale = Vector2.one * _data.damageRadius;
        _humanList.Clear();
        if (_flightCoroutine != null) StopCoroutine(_flightCoroutine);
        StartCoroutine(CoFlight());
    }

    private IEnumerator CoFlight()
    {
        float duration = _data.durationTime;
        float time = 0.0f;
        Vector3 start = transform.position;
        Vector3 end = _target;

        while (time < duration)
        {
            time += Time.deltaTime;
            float linearT = time / duration;
            float heightT = _curve.Evaluate(linearT);

            float height = Mathf.Lerp(0.0f, _data.maxHeight, heightT);

            transform.position = Vector2.Lerp(start, end, linearT) + new Vector2(0.0f, height);

            yield return null;
        }
        PoolManager.Instance.ReturnToPool(_data.projectileType.ToString(), this);
    }

    private void DestroyProjectile()
    {
        if (_destroyCoroutine != null) StopCoroutine(_destroyCoroutine);
        StartCoroutine(CoDestroy());
    }

    private IEnumerator CoDestroy()
    {
        yield return _animationTime;
        if (gameObject != null)
        {
            PoolManager.Instance.ReturnToPool(_data.projectileType.ToString(), this);
        }
    }
}
