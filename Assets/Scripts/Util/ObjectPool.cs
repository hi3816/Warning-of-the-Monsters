using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    private Dictionary<string, Queue<GameObject>> _poolDictionary;  // 각 풀을 관리하는 딕셔너리

    public void Initialize(Pool pool)
    {
        if (_poolDictionary == null)
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        }

        // 풀 딕셔너리에 이미 해당 태그와 일치하는 풀 있으면 리턴(중복 풀 생성 방지)
        if (_poolDictionary.ContainsKey(pool.tag))
        {
            Debug.Log($"Pool with tag {pool.tag} already exists.");
            return;
        }
        
        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            if (pool.prefab != null)
            {
                // 게임오브젝트 프리팹에서 생성하고 비활성화
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.name = pool.tag;
                // obj.name = pool.Tag + i;
                obj.SetActive(false);
                objectPool.Enqueue(obj);    // 큐 구조의 오브젝트풀에 생성된 게임오브젝트 추가
            }
        }
        
        _poolDictionary.Add(pool.tag, objectPool);  // 새로 만든 풀을 풀 딕셔너리에 추가
    }

    public GameObject SpawnFromPool(string tag)
    {
        // 풀 딕셔너리에 해당 태그와 일치하는 풀이 있는지 확인
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject obj = _poolDictionary[tag].Dequeue();    // 풀에서 가장 오래된 오브젝트 가져오기
        _poolDictionary[tag].Enqueue(obj);  // 다시 풀에 넣기(최신 오브젝트로 갱신)
        // 오브젝트 활성화하여 반환
        obj.SetActive(true);
        return obj;
    }
}