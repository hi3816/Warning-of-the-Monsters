using UnityEngine;

public class SetFormationNode : INode
{
    private HumanController _humanController;
    
    public SetFormationNode(HumanController humanController)
    {
        _humanController = humanController;
    }

    public NodeState Evaluate()
    {
        // Debug.Log($"Evaluating {this.GetType().Name}");

        _humanController.animator.SetBool("IsAttacked", true);

        Vector3 formationPosition = _humanController.human.transform.position;    // 각 인간 객체의 전투 시작 위치
        _humanController.MoveToFormationPosition(formationPosition);
        return NodeState.Failure; // TestCode 바로 상태 전환
    }
}