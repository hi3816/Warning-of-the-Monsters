using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [field:SerializeField] public List<Transform> SpawnPointList { get; private set; }

    private void Awake()
    {
        SpawnPointList = GetComponentsInChildren<Transform>().ToList();
        SpawnPointList.RemoveAt(0);
    }
}