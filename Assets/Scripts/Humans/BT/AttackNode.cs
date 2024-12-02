public class AttackNode : INode
{
    private HumanController _humanController;
    private bool _attackFlag;   // 공격을 수행했는지 확인

    public AttackNode(HumanController humanController)
    {
        _humanController = humanController;
        _attackFlag = false;
    }

    public NodeState Evaluate()
    {
        if (!_attackFlag)   // 공격한 적 없으면
        {
            _humanController.PerformAttack(); // 공격 수행
            _attackFlag = true; // 공격 수행 완료 상태로 전환
            return NodeState.Success; // WaitNode로
        }
        return NodeState.Failure;
    }

    public void Reset()
    {
        _attackFlag = false; // Reset state when re-entering the node
    }
}