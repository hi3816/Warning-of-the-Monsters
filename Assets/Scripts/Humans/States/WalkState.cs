using Unity.VisualScripting;
using UnityEngine;

public class WalkState : IHumanState
{
    private HumanController _humanController;

    public WalkState(HumanController humanController)
    {
        _humanController = humanController;
    }

    public void EnterState()
    {
        Debug.Log("Entering Walk State");

        if (_humanController.agent != null)
        {
            _humanController.agent.SetDestination(_humanController.endPoint.position);
        }
    }

    public void UpdateState()
    {
        if (_humanController.IsBattle)
        {
            _humanController.TransitionToState(new BattleState(_humanController));
            return;
        }

        if (_humanController.human.IsFearMaxed())
        {
            _humanController.TransitionToState(new RunState(_humanController));
            return;
        }

        if (_humanController.agent.remainingDistance <= _humanController.agent.stoppingDistance)
        {
            _humanController.ReturnHumanToPool();
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Walk State");
        _humanController.StopMoving();
    }
}