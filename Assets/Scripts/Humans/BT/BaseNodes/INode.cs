public enum NodeState { Success, Failure, Running }

public interface INode
{
    // 현재 노드 어떤 상태인지 반환
    NodeState Evaluate();
}