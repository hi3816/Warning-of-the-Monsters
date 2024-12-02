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
        if (_humanController.human.targetMonster == null)
        {
            return NodeState.Failure; // 타겟 몬스터 없으면 다른 상태로 전환
        }

        Debug.LogAssertion("Enter SetFormationNode");
        _humanController.animator.SetBool("IsAttacked", true);

        //Vector3 formationPosition = _humanController.human.targetMonster.transform.position;    // 각 인간 객체의 전투 시작 위치
        Vector3 formationPosition = _humanController.human.transform.position;    // 각 인간 객체의 전투 시작 위치
        if (_humanController.MoveToFormationPosition(formationPosition))
        {
            return NodeState.Running; // 전투 시작 위치에 도착할 때까지 유지
        }
        
        _humanController.RotateTowardsMonster(_humanController.human.targetMonster.transform.position); // 몬스터 위치로 방향 전환
        return NodeState.Success;   // 다른 상태로 전환
    }
}