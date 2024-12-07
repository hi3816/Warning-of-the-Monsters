using UnityEngine;

public class HumanBaseHumanState : IHumanState
{
    protected HumanStateMachine stateMachine;

    public HumanBaseHumanState(HumanStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}