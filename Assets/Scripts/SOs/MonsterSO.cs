using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO_", menuName = "MonsterSO_")]
public class MonsterSO : ScriptableObject
{
    public int id;
    public float monsterId;
    public int upgradeLevel;
    public string poolTag;
    public float fatigue; //몬스터 피로도바 최대치
    public float minFearInflicted; //적(인간)에게 주는 공포수치량
    public float maxFearInflicted;
    public float cooldown; //몬스터 놀래킴 쿨타임
    public float humanDetectRange; //적(인간)을 탐지할 수 있는 범위
    public float humanScaringRange; //적(인간)을 놀래킬 수 있는 범위
    public float walkSpeed; //미니언 걷는 속도
    public int requiredCoins; //필요재화
    public int maxLevel; // 최대 레벨 -> 진화
    public MonsterType monsterType;
    public int summonerId;
}

public class SummonSO : ScriptableObject
{
    public float monsterID;
    public EvolutionType evolutionType;
    public List<int> minionList;
    public List<string> minionTagList;
    public List<int> minionCountList;
    public List<int> evolutionMinionIdList;
    public List<string> evolutionMinionTagList;
    public List<int> eovlutionMinionCountList;
}