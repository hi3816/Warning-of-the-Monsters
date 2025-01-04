using DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private Human _human;
    private MonsterData _monsterData;
    private Projectile_Data _data;
    private Vector2 _direction;
    private bool _isStart = false;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Coroutine coroutine;
    private WaitForSeconds _animationTime;
    private List<GameObject> _humanList = new List<GameObject>();

    private void Awake()
    {
        _humanList.Clear();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _animationTime = new WaitForSeconds(stateInfo.length * 1.5f);
    }

    private void Update()
    {
        if (!_isStart)
        {
            return;
        }

        _rigidbody.velocity = _direction * _data.speed;
        //_rigidbody.AddForce(transform.forward * 45f, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            Human human = collision.GetComponent<Human>();
            
            if (_humanList.Contains(collision.gameObject)) return;

            // 리스트에 없으면 추가하고 스킬 공격력 만큼 피해 입히기
            _humanList.Add(collision.gameObject);

            human.IncreaseFear(Random.Range(_monsterData.minFearInflicted, _monsterData.maxFearInflicted));
            if (coroutine != null) StopCoroutine(coroutine);
            StartCoroutine(CoDestroy());
        }
    }

    public void SetProjectileInfo(Vector2 target, MonsterData monsterData, Projectile_Data projectileData)
    {
        //Vector2 pos = transform.position + (Random.insideUnitSphere * _data.damageRadius * 0.5f);
        _monsterData = monsterData;
        _direction = GetDirection(target, transform.position);
        _data = projectileData;
        transform.localScale = Vector2.one * _data.damageRadius;
        //transform.rotation = Quaternion.LookRotation(_direction).normalized;
        _isStart = true;
        _humanList.Clear();
    }

    private Vector2 GetDirection(Vector2 target, Vector3 current)
    {
        Vector2 curr = current;
        Vector2 direction = (target - curr).normalized;
        return direction;
    }

    private float GetDistance(Vector2 target, Vector3 current)
    {
        Vector2 curr = current;
        float distance = Vector2.Distance(target, curr);
        return distance;
    }

    private Vector2 GetNewTarget(Vector2 target)
    {
        Vector2 offset = Random.insideUnitSphere;
        Vector2 newTarget = target + (offset * _data.damageRadius * 0.5f);
        return newTarget;
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
