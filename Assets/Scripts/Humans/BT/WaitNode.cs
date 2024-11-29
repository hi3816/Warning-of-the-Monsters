public class WaitNode : INode
{
    private HumanController _humanController;

    public WaitNode(HumanController humanController)
    {
        _humanController = humanController;
    }

    public NodeState Evaluate()
    {
        if (_humanController.CanAttack())
        {
            return NodeState.Success; // Attack 상태로 전환
        }
        return NodeState.Running; // 쿨타임 찰 때까지 유지
    }
}