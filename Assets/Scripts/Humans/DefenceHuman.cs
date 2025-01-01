using System;
using UnityEngine;

public class DefenceHuman : Human
{
    private float _defenceChance;
    
    protected override void Awake()
    {
        base.Awake();
        _defenceChance = controller.Cooldown;
    }

    public override void IncreaseFear(float amount)
    {
        float randNum = UnityEngine.Random.Range(0f, 10f);
        if (randNum < _defenceChance)
        {
            controller.animator.SetTrigger("Defence");
            return;
        }
        base.IncreaseFear(amount);
    }
}