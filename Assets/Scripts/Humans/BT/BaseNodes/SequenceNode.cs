using System.Collections.Generic;

// 자식 노드를 순서대로 진행하면서 Failure 상태가 나올 때까지 진행하는 노드
public class SequenceNode : INode
{
    private readonly List<INode> _children = new List<INode>();

    public void AddChild(INode child)
    {
        _children.Add(child);
    }

    public NodeState Evaluate()
    {
        if (_children == null || _children.Count == 0)
            return NodeState.Failure;

        foreach (INode child in _children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success:
                    continue;   // 자식 상태 Success 면 다음 자식으로 이동
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failure:
                    return NodeState.Failure;
            }
        }
        return NodeState.Failure;
    }
}