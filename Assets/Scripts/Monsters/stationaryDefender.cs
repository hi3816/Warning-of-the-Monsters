using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stationaryDefender : Monster //한자리를 지키고 있는 몬스터(=일반타워)
{
    
    void Update()
    {
        base.Update();
    }

    protected override void Scaring(float time)
    {
        base.Scaring(time);
       
        // 범위 공격
        // if (Time.time - _lastScareTime > data.cooldown)
        // {
        //     foreach (Human human in _targetHumanList)
        //     {
        //         if (human == null) continue;
        //
        //         human.IncreaseFear(data.fearInflicted);
        //         //human.controller.SetTargetMonster(transform);
        //     }
        //     _lastScareTime = Time.time;
        // }

        if (TargetHumanList.Count == 0)
        {
            SetState(MonsterState.Idle);
        }
    }
}
