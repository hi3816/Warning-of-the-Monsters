using GoogleSheet.Core.Type;
using JetBrains.Annotations;
using UnityEngine;


[UGS(typeof(SkillName))]

public enum SkillName
{
    MonsterRoar,
    FrozenGround,
    Bomb,
}

[UGS(typeof(SkillType))]
public enum SkillType
{
    Attack,
    Debuff,
}