using UnityEngine;

public class AttackNode : INode
{
    private HumanController _humanController;

    public AttackNode(HumanController humanController)
    {
        _humanController = humanController;
    }

    public NodeState Evaluate()
    {
        _humanController.nodeTxt.text = "Attack";
        // Debug.Log($"Evaluating {this.GetType().Name}");
        if (!_humanController.HasTargetMonster())   // 타겟몬스터 없어지면
        {
            return NodeState.Failure;
        }
        if (_humanController.CanAttack())
        {
            //Debug.LogAssertion("Attack Check");
            _humanController.PerformAttack(); // 공격 수행
        }
        return NodeState.Success; 
    }
}