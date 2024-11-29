using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    public HumanSO humanData;
    public Animator animator;
    public Monster targetMonster;

    private NavMeshAgent agent;

    public bool IsWaveStarted { get; set; }
    public float FearLevel { get; set; }
}
