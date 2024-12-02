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

    private void Awake()
    {
        // 카메라 상에 캐릭터가 보이도록 축과 회전값 조정
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
}
