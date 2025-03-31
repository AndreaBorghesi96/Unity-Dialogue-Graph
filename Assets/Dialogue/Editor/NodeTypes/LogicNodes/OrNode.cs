public class OrNode : LogicNode
{
    public OrNode(DialogueNodeData data = null)
    {
        title = "OR";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
    }
}