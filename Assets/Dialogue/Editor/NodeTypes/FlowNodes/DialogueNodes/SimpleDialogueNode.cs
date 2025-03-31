public class SimpleDialogueNode : DialogueNode
{
    public SimpleDialogueNode(DialogueNodeData data = null)
    {
        title = "Dialogue Node";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
        DialogueText = data != null ? data.DialogueText : "Text";
    }
}
