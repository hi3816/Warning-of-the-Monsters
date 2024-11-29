using System;

// 조건문 확인 후 true면 하위 노드 실행, false면 failure 반환
public class ConditionNode : INode
{
    private INode _node;
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition, INode node)
    {
        _condition = condition;
        _node = node;
    }

    public NodeState Evaluate()
    {
        bool result = _condition.Invoke();
        return result ? _node.Evaluate() : NodeState.Failure;
    }
}