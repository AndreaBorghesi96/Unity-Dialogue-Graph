using System.Collections.Generic;
using UnityEngine;

public class EventNode : FlowNode
{
    public List<ScriptableObject> events;
    public EventNode(DialogueNodeData data = null)
    {
        title = "Event Node";
        Guid = data != null ? data.Guid : System.Guid.NewGuid().ToString();
        events = data != null ? data.Events : new List<ScriptableObject>();
    }
}