using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO", menuName = "MonsterSO")]
public class MonsterSO : ScriptableObject
{
    [SerializeField] private float fatigue; //몬스터 피로도
    [SerializeField] private float fearInflicted; //적(인간)에게 주는 공포수치
    [SerializeField] private float cooldown; //쿨타임
    [SerializeField] private float requiredCoins; //필요 재화
}