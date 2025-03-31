public class AndNode : LogicNode
{
    
    public AndNode(DialogueNodeData data = null)
    {
        title = "AND";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
    }
}