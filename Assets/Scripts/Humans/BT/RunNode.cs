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
        // 도착 지점에 갈 때까지 계속 Run 상태 유지
        if (_humanController.MoveToDestination(_spawnPosition))
        {
            return NodeState.Running;
        }
        return NodeState.Success; // 도착 지점 도달하면 다른 상태로 전환
    }
}