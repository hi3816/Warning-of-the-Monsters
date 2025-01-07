using GoogleSheet.Core.Type;
using UnityEngine;

[UGS(typeof(EvolutionType))]
public enum EvolutionType // 몬스터 진화 타입
{
    Atype, // 진화 시 왼쪽 몬스터
    Btype, // 진화 시 오른쪽 몬스터
}

[UGS(typeof(MonsterType))]

public enum MonsterType
{
    Stationary,
    Summoner,
    Minion,
}

[UGS(typeof(ProjectileType))]

public enum ProjectileType
{
    BigRock, // 큰 바위 (추후 작은 바위 추가)
    Frostbolt, // 눈
}