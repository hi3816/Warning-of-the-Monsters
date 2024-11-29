using System;

// 실제로 행위를 하는 노드
public class ActionNode : INode
{
    private Func<NodeState> _onUpdate;

    public ActionNode(Func<NodeState> onUpdate)
    {
        _onUpdate = onUpdate;
    }
    
    public NodeState Evaluate() => _onUpdate?.Invoke() ?? NodeState.Failure;
}