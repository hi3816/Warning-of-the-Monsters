using UnityEngine;

[CreateAssetMenu(fileName = "HumanSO", menuName = "HumanSO")]
public class HumanSO : ScriptableObject
{
    public int id;                    // 인간 종류에 따른 식별 id
    public float maxFear;             // 최대 공포 수치
    public float minFatigueInflicted; // 몬스터의 피로도(체력) 최소 증가치
    public float maxFatigueInflicted; // 몬스터의 피로도(체력) 최대 증가치
    public float cooldown;            // 공격 쿨타임
    public float speed;               // 속도
    public int lifeInflicted;         // 스테이지의 health 감소치
    public int coin;                // 처치 시 플레이어에 지급되는 재화량
}