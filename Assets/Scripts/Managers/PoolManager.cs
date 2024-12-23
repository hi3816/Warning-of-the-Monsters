using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PoolManager : SingletonBase<PoolManager>
{
    // Inspector에서 여러 풀 한번에 관리
    [System.Serializable]
    public class PoolConfig
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();
    private Dictionary<string, ObjectPool> _pools = new Dictionary<string, ObjectPool>();

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
        DontDestroyOnLoad(gameObject);
    }

    private void InitializePools()
    {
        foreach (var config in poolConfigs)
        {
            CreatePool(config.tag, config.prefab, config.size);
        }
    }

    private void CreatePool(string tag, GameObject prefab, int size)
    {
        // 풀 딕셔너리에 이미 해당 태그와 일치하는 풀 있으면 리턴(중복 풀 생성 방지)
        if (_pools.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} already exists.");
            return;
        }

        /* 계층 구조 생성하여 정리 */
        // PoolManager
        // - Pool_XXX
        // -- GameObject(Clone)
        // -- GameObject(Clone)...
        GameObject poolObject = new GameObject($"Pool_{tag}");  // 풀 관리할 빈 게임오브젝트 생성하고 태그로 이름 구별
        poolObject.transform.SetParent(transform);  // PoolManager의 자식으로 설정

        // Inspector에서 받아온 설정 정보 기반으로 새로운 오브젝트 풀 생성
        ObjectPool objectPool = poolObject.AddComponent<ObjectPool>();
        objectPool.Initialize(new ObjectPool.Pool
        {
            Tag = tag,
            Prefab = prefab,
            Size = size
        });

        _pools.Add(tag, objectPool);    // 풀 딕셔너리에 새로운 오브젝트 풀 추가
    }

    public void AddPoolS(PoolConfig[] pools)
    {
        foreach(PoolConfig pool in pools)
        {
            poolConfigs.Add(pool);
            CreatePool(pool.tag, pool.prefab, pool.size);
        }
    }

    public void AddPool(PoolConfig pool)
    {
        poolConfigs.Add(pool);
        CreatePool(pool.tag, pool.prefab, pool.size);
    }

    // Transform 설정하는 SpawnFromPool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // 태그와 일치하는 풀이 있는지 유효성 검사
        if (!_pools.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject obj = _pools[tag].SpawnFromPool(tag);    // 풀에서 오브젝트 가져오기
        // Object 있으면 Transform 설정하고 반환
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;
    }
    
    // 게임오브젝트만 반환하는 SpawnFromPool
    public GameObject SpawnFromPool(string tag)
    {
        // 태그와 일치하는 풀이 있는지 유효성 검사
        if (!_pools.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        // 풀에서 오브젝트 가져와 반환
        GameObject obj = _pools[tag].SpawnFromPool(tag);
        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (obj == null) return;
        // 태그와 일치하는 풀이 있는지 유효성 검사
        if (!_pools.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} doesn't exist.");
            return;
        }
        obj.SetActive(false);   // 오브젝트 비활성화
    }
    
    // 특정 태그의 오브젝트 풀을 삭제
    public void DeletePool(string tag)
    {
        // 태그와 일치하는 풀이 있는지 유효성 검사
        if (!_pools.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} doesn't exist.");
            return;
        }

        Destroy(_pools[tag].gameObject);    // 오브젝트 풀 삭제
        _pools.Remove(tag); // 풀 목록에서 태그 제거

        // 인자로 들어온 tag가 PoolConfig의 tag와 일치하면 해당 PoolConfig 삭제
        poolConfigs.RemoveAll(config => config.tag == tag);

        Debug.Log($"Pool with tag {tag} deleted successfully.");
        
        EditorUtility.SetDirty(this);   // Asset 상태 갱신 에디터에 전달
    }

    // 풀 딕셔너리에 등록된 모든 오브젝트 풀 삭제
    public void DeleteAllPools()
    {
        if (_pools == null) return;
        
        // 풀 딕셔너리에 있는 오브젝트 풀로 생성된 게임오브젝트 삭제
        foreach (var pool in _pools.Values)
        {
            Destroy(pool.gameObject);
        }

        _pools.Clear();         // 풀 딕셔너리에서 모든 항목 삭제
        poolConfigs.Clear();    // 풀 설정 리스트에서 모든 항목 삭제

        Debug.Log("All pools have been deleted.");

        EditorUtility.SetDirty(this);   // Asset 상태 갱신 에디터에 전달
    }
}
