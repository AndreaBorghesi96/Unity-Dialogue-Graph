public class ChoiceDialogueNode : DialogueNode
{
   
    public ChoiceDialogueNode(DialogueNodeData data = null)
    {
        title = "Multiple Choice Node";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
        DialogueText = data != null ? data.DialogueText : "Text";
    } 
}
