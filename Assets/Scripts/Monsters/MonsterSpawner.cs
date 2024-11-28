using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Transform Stage1;
    public string poolTag;
    private Transform[] spawnPoints;
    private bool[] isOccupied;

    void Start()
    {
        spawnPoints = Stage1.GetComponentsInChildren<Transform>();
        isOccupied = new bool[spawnPoints.Length];
    }
}