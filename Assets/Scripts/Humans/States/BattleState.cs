using UnityEngine;

public class BattleState : IHumanState
{
    private HumanController _humanController;

    public BattleState(HumanController humanController)
    {
        _humanController = humanController;
    }

    public void EnterState()
    {
        Debug.Log("Entering Battle State");

        if (_humanController.targetMonster != null && _humanController.agent != null)
        {
            _humanController.agent.SetDestination(_humanController.targetMonster.transform.position);
        }
    }

    public void UpdateState()
    {
        if (_humanController.targetMonster == null)
        {
            _humanController.TransitionToState(new WalkState(_humanController));
            return;
        }

        if (_humanController.human.IsFearMaxed())
        {
            _humanController.TransitionToState(new RunState(_humanController));
        }

        if (_humanController.agent.remainingDistance <= _humanController.agent.stoppingDistance)
        {
            if (_humanController.CanAttack())
            {
                _humanController.PerformAttack();
            }
        }
        else
        {
            _humanController.agent.SetDestination(_humanController.targetMonster.transform.position);
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Battle State");
        _humanController.StopMoving();
        _humanController.ResetTarget();
    }
}