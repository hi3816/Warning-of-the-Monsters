using System.Collections.Generic;

// 자식 노드에서 처음으로 Success나 Running 상태 노드 발생하면
// 그 노드까지 진행하고 멈추는 노드
public class SelectorNode : INode
{
    private List<INode> _children = new List<INode>();

    public void AddChild(INode child)
    {
        _children.Add(child);
    }

    public NodeState Evaluate()
    {
        if (_children == null)  return NodeState.Failure;
        
        foreach (INode child in _children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
                // 자식 상태가 Failure 면 다음 자식으로 이동
            }
        }
        return NodeState.Failure;
    }
}