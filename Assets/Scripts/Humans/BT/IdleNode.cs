public class IdleNode : INode
{
    private HumanController _humanController;

    public IdleNode(HumanController humanController)
    {
        _humanController = humanController;
    }

    public NodeState Evaluate()
    {
        // Debug.Log($"Evaluating {this.GetType().Name}");
        if (!_humanController.IsWaveStarted())  // 웨이브 시작 전이면
        {
            return NodeState.Running; // Idle 상태를 유지
        }
        return NodeState.Failure; // 웨이브 시작하면 다른 상태로 전환
    }
}