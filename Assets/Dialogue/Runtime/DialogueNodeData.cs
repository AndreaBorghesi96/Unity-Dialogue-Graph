using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public string NodeType;
    public string DialogueText;
    public List<ScriptableObject> Events;
    public Vector2 Position;

    public List<PortData> OutputPortList = new List<PortData>();

    public List<PortData> InputPortList = new List<PortData>();

    public PortData GetFlowEntryPort()
    {
        List<PortData> entryPorts = InputPortList.FindAll(port => port.Type == PortType.Flow);
        return entryPorts.Count() > 0 ? entryPorts[0] : null;
    }

    public PortData GetFlowExitPort()
    {
        List<PortData> exitPorts = OutputPortList.FindAll(port => port.Type == PortType.Flow);
        return exitPorts.Count() > 0 ? exitPorts[0] : null;
    }

    public List<PortData> GetFlowExitPorts()
    {
        return OutputPortList.FindAll(port => port.Type == PortType.Flow);
    }
    public PortData GetLogicExitPort()
    {
        List<PortData> exitPorts = OutputPortList.FindAll(port => port.Type == PortType.Logic);
        return exitPorts.Count() > 0 ? exitPorts[0] : null;
    }
    public PortData GetLogicEntryPort()
    {
        List<PortData> entryPorts = InputPortList.FindAll(port => port.Type == PortType.Logic);
        return entryPorts.Count() > 0 ? entryPorts[0] : null;
    }
    public PortData GetTrueExitPort()
    {
        List<PortData> exitPorts = OutputPortList.FindAll(port => port.Type == PortType.Flow && port.Text == "True");
        return exitPorts.Count() > 0 ? exitPorts[0] : null;
    }
    public PortData GetFalseExitPort()
    {
        List<PortData> exitPorts = OutputPortList.FindAll(port => port.Type == PortType.Flow && port.Text == "False");
        return exitPorts.Count() > 0 ? exitPorts[0] : null;
    }
}