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
        // Debug.Log($"Evaluating {this.GetType().Name}");
        _humanController.nodeTxt.text = "Walk";

        if (_humanController.HasTargetMonster() || _humanController.IsFearMaxed())   // 타겟몬스터 있으면
        {
            _humanController.animator.SetBool("IsBattle", true);
            _humanController.animator.SetBool("IsWalk", false);
            return NodeState.Failure;
        }
        if (!_humanController.ArriveToDestination(_targetPosition))
        {
            _humanController.animator.SetBool("IsWalk", true);
            return NodeState.Running;   // 목적지에 도착할 때까지 계속 유지
        }
        _humanController.animator.SetBool("IsWalk", true);
        return NodeState.Success;   // 목적지에 도달하면 종료
    }
}