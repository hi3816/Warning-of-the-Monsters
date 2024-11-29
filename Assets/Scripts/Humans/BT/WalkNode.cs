using UnityEngine;

public class WalkNode : INode
{
    private HumanController _humanController;
    private Vector3 _targetPosition;

    public WalkNode(HumanController humanController, Vector3 targetPosition)
    {
        _humanController = humanController;
        _targetPosition = targetPosition;
    }

    public NodeState Evaluate()
    {
        if (_humanController.MoveToDestination(_targetPosition))
        {
            return NodeState.Running;   // 목적지에 도착할 때까지 계속 유지
        }
        return NodeState.Success;   // 목적지에 도달하면 종료
    }
}