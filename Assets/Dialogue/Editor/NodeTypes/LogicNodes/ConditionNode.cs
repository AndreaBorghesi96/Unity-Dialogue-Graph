public class ConditionNode : LogicNode
{
    
    public ConditionNode(DialogueNodeData data = null)
    {
        title = "Condition";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
    }
}