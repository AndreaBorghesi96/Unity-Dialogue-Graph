using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();

    public bool ValidateDialogue()
    {
        bool noDeadNodes = true;
        foreach (var node in DialogueNodeData)
        {
            List<string> exitPortGuids = node.GetFlowExitPorts().Select(p => p.Guid).ToList();
            foreach (var g in exitPortGuids)
            {
                if (!NodeLinks.Select(l => l.BasePortGuid).Contains(g))
                {
                    noDeadNodes = false;
                }
            }
        }

        return DialogueNodeData.FindAll(n => n.NodeType == "EntryNode").Count() == 1 &&
            DialogueNodeData.FindAll(n => n.NodeType == "EndNode").Count() == 1 &&
            noDeadNodes;
    }

    public DialogueNodeData GetEntryNode()
    {
        return DialogueNodeData.FindAll(n => n.NodeType == "EntryNode")[0];
    }
}