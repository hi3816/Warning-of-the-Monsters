using Human_Data;
using UnityEngine;

public static class ConvertUtility
{
    public static HumanSO Convert(this HumanData data)
    {
        var so = ScriptableObject.CreateInstance<HumanSO>();
        so.id = data.id;
        so.name = $"HumanSO{data.id + 1}";
        so.maxFear = data.maxFear;
        so.minFatigueInflicted = data.minFatigueInflicted;
        so.maxFatigueInflicted = data.maxFatigueInflicted;
        so.cooldown = data.cooldown;
        so.speed = data.speed;
        so.lifeInflicted = data.lifeInflicted;
        so.coin = data.coin;
        
        return so;
    }
}
