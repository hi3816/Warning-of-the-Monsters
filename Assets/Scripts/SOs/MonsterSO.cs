using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO_", menuName = "MonsterSO_")]
public class MonsterSO : ScriptableObject
{
    public int id;
    public int minionId;
    public float monsterId;
    public int upgradeLevel;
    public string poolTag;
    public float fatigue; //몬스터 피로도바 최대치
    public float minFearInflicted; //적(인간)에게 주는 공포수치량
    public float maxFearInflicted;
    public float cooldown; //몬스터 놀래킴 쿨타임
    public float humanDetectRange;
    public float humanScaringRange; //적(인간)을 놀래킬 수 있는 범위
    public float walkSpeed; //미니언 걷는 속도
    public int requiredCoins; //필요재화
    public int maxLevel; // 최대 레벨 -> 진화
}