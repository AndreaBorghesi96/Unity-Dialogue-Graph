public class ConditionNode : LogicNode
{
    public string VarName; 
    public ConditionNode(DialogueNodeData data = null)
    {
        title = "Condition";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
        VarName = data != null ? data.VarName : "";
    }
}