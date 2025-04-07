public class DialogueBrowser
{
    private DialogueContainer dialogueGraph;
    private DialogueNodeData currentNode;
    public DialogueNodeData CurrentNode
    {
        get => currentNode;
    }

    public DialogueBrowser(DialogueContainer dialogue)
    {
        if(!dialogue.ValidateDialogue()) return;
        dialogueGraph = dialogue;
        currentNode = dialogueGraph.GetEntryNode();
    }

    public DialogueNodeData NextNode(string portGuid)
    {
        if(currentNode.NodeType == "EndNode") return currentNode;
        NodeLinkData chosenLink = dialogueGraph.NodeLinks.Find(l => l.BasePortGuid == portGuid);
        currentNode = dialogueGraph.DialogueNodeData.Find(n => n.Guid == chosenLink.TargetNodeGuid);
        return currentNode;
    }
}