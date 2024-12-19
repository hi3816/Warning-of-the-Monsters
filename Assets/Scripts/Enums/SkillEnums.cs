using GoogleSheet.Core.Type;
using JetBrains.Annotations;
using UnityEngine;


[UGS(typeof(SkillName))]

public enum SkillName
{
    MonsterRoar,
    FrozenGround,
}

[UGS(typeof(SkillType))]
public enum SkillType
{
    Attack,
    Debuff,
}