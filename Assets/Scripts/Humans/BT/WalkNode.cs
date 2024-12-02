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
        if (_humanController.human.targetMonster != null)   // 타겟몬스터 있으면
        {
            Debug.LogAssertion("Human surprised by monster. Change Walk to Surprise");
            return NodeState.Failure; // Surprise로 전환
        }
        if (_humanController.MoveToDestination(_targetPosition))
        {
            _humanController.animator.SetTrigger("Walk");
            return NodeState.Running;   // 목적지에 도착할 때까지 계속 유지
        }
        return NodeState.Success;   // 목적지에 도달하면 종료
    }
}