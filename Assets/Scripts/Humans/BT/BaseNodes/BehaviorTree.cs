public class BehaviorTree
{
    private readonly INode root;

    public BehaviorTree(INode rootNode)
    {
        root = rootNode;
    }

    public void Update()
    {
        root.Evaluate();
    }
}