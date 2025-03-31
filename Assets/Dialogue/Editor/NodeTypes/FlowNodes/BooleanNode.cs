public class BooleanNode : FlowNode
{
    
    public BooleanNode(DialogueNodeData data = null)
    {
        title = "IF";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
    }
}