using UnityEngine;

[CreateAssetMenu(fileName = "HumanSO", menuName = "HumanSO")]
public class HumanSO : ScriptableObject
{
    [SerializeField] private float maxFear;             // 최대 공포 수치
    [SerializeField] private float minFatigueInflicted; // 몬스터의 피로도(체력) 최소 감소치
    [SerializeField] private float maxFatigueInflicted; // 몬스터의 피로도(체력) 최대 감소치
    [SerializeField] private float cooldown;            // 공격 쿨타임
    [SerializeField] private float speed;               // 속도
    [SerializeField] private float lifeInflicted;       // 스테이지의 health 감소치
    [SerializeField] private float coin;                // 처치 시 플레이어에 지급되는 재화량
}