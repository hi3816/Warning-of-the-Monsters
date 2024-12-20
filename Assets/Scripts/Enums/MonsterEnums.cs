using GoogleSheet.Core.Type;
using UnityEngine;

[UGS(typeof(EvolutionType))]
public enum EvolutionType // 몬스터 진화 타입
{
    Atype,
    Btype,
}

[UGS(typeof(MonsterType))]

public enum MonsterType
{
    Stationary,
    Summoner,
    Minion,
}