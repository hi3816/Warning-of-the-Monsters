using UnityEngine;

public class RunNode : INode
{
    private HumanController _humanController;
    private Vector3 _spawnPosition;
    
    public RunNode(HumanController humanController, Vector3 spawnPosition)
    {
        _humanController = humanController;
        _spawnPosition = spawnPosition;
    }

    public NodeState Evaluate()
    {
        // Debug.Log($"Evaluating {this.GetType().Name}");
        if (_humanController.IsFearMaxed())
        {
            _humanController.ArriveToDestination(_spawnPosition);
            return NodeState.Running;
        }
        return NodeState.Failure;
    }
}