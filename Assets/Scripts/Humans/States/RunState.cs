using UnityEngine;

public class RunState : IHumanState
{
    private HumanController _humanController;

    public RunState(HumanController humanController)
    {
        _humanController = humanController;
    }

    public void EnterState()
    {
        Debug.Log("Entering Run State");

        if (_humanController.agent != null)
        {
            _humanController.agent.SetDestination(_humanController.spawnPoint.position);
        }
    }

    public void UpdateState()
    {
        _humanController.ReturnHumanToPool();
    }

    public void ExitState()
    {
        Debug.Log("Exiting Run State");
        _humanController.StopMoving();
    }
}