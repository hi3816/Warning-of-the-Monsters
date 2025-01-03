using DataTable;
using System.Collections;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private int _projectileId;
    private Projectile_Data _data;
    private Vector2 _direction;
    private bool _isStart = false;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Coroutine coroutine;
    private WaitForSeconds _animationTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _animationTime = new WaitForSeconds(stateInfo.length * 2f);
    }

    private void Update()
    {
        if (!_isStart)
        {
            return;
        }

        _rigidbody.velocity = _direction * _data.speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject != null && collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            PoolManager.Instance.ReturnToPool(_data.projectileType.ToString(), transform.gameObject);
        }
    }

    public void SetProjectileInfo(Vector2 target, Projectile_Data projectileData)
    {
        //Vector2 pos = transform.position + (Random.insideUnitSphere * _data.damageRadius * 0.5f);
        Vector2 pos = transform.position;
        _direction = target - pos;
        _data = projectileData;
        transform.localScale = Vector2.one * _data.damageRadius;
        _isStart = true;

        if (coroutine != null) StopCoroutine(coroutine);
        StartCoroutine(CoDestroy());
    }

    private IEnumerator CoDestroy()
    {
        yield return _animationTime;
        if (gameObject != null)
        {
            PoolManager.Instance.ReturnToPool(_data.projectileType.ToString(), transform.gameObject);
        }
    }
}
