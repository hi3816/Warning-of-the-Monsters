using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO_", menuName = "MonsterSO_")]
public class MonsterSO : ScriptableObject
{
    public GameObject prefab;
    public string poolTag;
    public float fatigue; //몬스터 피로도바 최대치
    public float fearInflicted; //적(인간)에게 주는 공포수치량
    public float cooldown; //몬스터 놀래킴 쿨타임
    public float humanScaringRange; //적(인간)을 놀래킬 수 있는 범위
    public float requiredCoins; //필요재화
}