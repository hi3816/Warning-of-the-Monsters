using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO_", menuName = "MonsterSO_")]
public class MonsterSO : ScriptableObject
{
    public float fatigue; //몬스터 피로도바 최대치
    public float fearInflicted; //적(인간)에게 주는 공포수치량
    public float initialCooldown; //쿨타임
    public float cooldown; //쿨타임
    public float humanDetectionRange; //인간탐지범위
    public float humanScaringRange; //인간을 놀래킬 수 있는 범위
    public float requiredCoins; //필요재화
}