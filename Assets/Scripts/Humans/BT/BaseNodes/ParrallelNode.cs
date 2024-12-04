using System.Collections.Generic;

public class ParallelNode : INode
{
    private List<INode> _children;
    private int _successThreshold;
    private int _failureThreshold;

    public ParallelNode(List<INode> children, int successThreshold = -1, int failureThreshold = -1)
    {
        _children = children;
        _successThreshold = successThreshold > 0 ? successThreshold : _children.Count;
        _failureThreshold = failureThreshold > 0 ? failureThreshold : 1;
    }

    public NodeState Evaluate()
    {
        int successCount = 0;
        int failureCount = 0;

        foreach (var child in _children)
        {
            var state = child.Evaluate();

            if (state == NodeState.Success)
                successCount++;
            else if (state == NodeState.Failure)
                failureCount++;
        }

        if (successCount >= _successThreshold)
            return NodeState.Success;

        if (failureCount >= _failureThreshold)
            return NodeState.Failure;

        return NodeState.Running;
    }
}