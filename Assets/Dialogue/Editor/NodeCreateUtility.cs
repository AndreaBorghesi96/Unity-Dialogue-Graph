using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public static class NodeCreateUtility
{
    private static Port GeneratePort(BaseNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    public static void AddFlowEntryPort(FlowNode node, PortData portData = null)
    {
        var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        inputPort.userData = portData != null ? portData : new PortData(PortType.Flow);
        node.inputContainer.Add(inputPort);
    }
    public static void AddFlowExitPort(FlowNode node, PortData portData = null)
    {
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Output";
        generatedPort.userData = portData != null ? portData : new PortData(PortType.Flow);
        node.outputContainer.Add(generatedPort);
    }
    public static void AddCustomFlowExitPorts(FlowNode node, DialogueGraphView graphView, List<PortData> portData = null)
    {
        if (portData == null || portData.Count() == 0)
        {
            AddCustomFlowExitPort(node, graphView, false);
            return;
        }

        for (var i = 0; i < portData.Count(); i++)
        {
            var generatedPort = GeneratePort(node, Direction.Output);
            var generatedPortName = portData[i].Text;

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            oldLabel.style.display = DisplayStyle.None;

            var textField = new TextField
            {
                value = portData[i].Text,
                multiline = true
            };
            textField.style.width = 100f;
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.RegisterValueChangedCallback(evt =>
            {
                generatedPort.portName = evt.newValue;
                ((PortData)generatedPort.userData).Text = evt.newValue;
            });
            generatedPort.portName = generatedPortName;
            generatedPort.userData = portData[i];

            generatedPort.contentContainer.Add(textField);

            if (i > 0)
            {
                var deleteButton = new Button(() => RemovePort(node, generatedPort, graphView))
                {
                    text = "X"
                };
                generatedPort.contentContainer.Add(deleteButton);
            }

            node.outputContainer.Add(generatedPort);
        }

        node.RefreshPorts();
        node.RefreshExpandedState();
    }
    public static void AddCustomFlowExitPort(FlowNode node, DialogueGraphView graphView, bool deletable = true)
    {
        var generatedPort = GeneratePort(node, Direction.Output);
        var generatedPortName = "Text";

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        oldLabel.style.display = DisplayStyle.None;

        var textField = new TextField
        {
            value = generatedPortName,
            multiline = true
        };
        textField.style.width = 100f;
        textField.style.whiteSpace = WhiteSpace.Normal;
        textField.RegisterValueChangedCallback(evt =>
        {
            generatedPort.portName = evt.newValue;
            ((PortData)generatedPort.userData).Text = evt.newValue;
        });
        generatedPort.portName = generatedPortName;
        generatedPort.userData = new PortData(generatedPortName, PortType.Flow);

        generatedPort.contentContainer.Add(textField);

        if (deletable)
        {
            generatedPort.contentContainer.Add(new Button(() => RemovePort(node, generatedPort, graphView))
            {
                text = "X"
            });
        }

        node.outputContainer.Add(generatedPort);
    }
    public static void AddLogicExitPort(LogicNode node, PortData portData = null)
    {
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Result";
        generatedPort.userData = portData != null ? portData : new PortData(PortType.Logic);
        node.outputContainer.Add(generatedPort);
    }
    public static void AddBooleanExitPorts(BooleanNode node, PortData truePortData = null, PortData falsePportData = null)
    {
        var generatedPortTrue = GeneratePort(node, Direction.Output);
        generatedPortTrue.portName = "True";
        generatedPortTrue.userData = truePortData != null ? truePortData : new PortData("True", PortType.Flow);
        node.outputContainer.Add(generatedPortTrue);
        var generatedPortFalse = GeneratePort(node, Direction.Output);
        generatedPortFalse.portName = "False";
        generatedPortFalse.userData = falsePportData != null ? falsePportData : new PortData("False", PortType.Flow);
        node.outputContainer.Add(generatedPortFalse);
    }
    public static void AddLogicEntryPort(BaseNode node, Port.Capacity capacity, PortData portData = null)
    {
        var inputPort = GeneratePort(node, Direction.Input, capacity);
        inputPort.portName = capacity == Port.Capacity.Multi ? "Conditions" : "Condition";
        inputPort.userData = portData != null ? portData : new PortData(PortType.Logic);
        node.inputContainer.Add(inputPort);
    }
    public static void AddTextBox(DialogueNode node)
    {
        var textField = new TextField
        {
            value = string.Empty,
            multiline = true
        };

        textField.style.minWidth = 200f;
        textField.style.width = StyleKeyword.Auto;
        textField.style.whiteSpace = WhiteSpace.Normal;

        textField.RegisterValueChangedCallback(evt =>
        {
            node.DialogueText = evt.newValue;
        });
        textField.SetValueWithoutNotify(node.DialogueText != null ? node.DialogueText : "Text");
        node.mainContainer.Add(textField);
    }
    private static void RemovePort(BaseNode dialogueNode, Port generatedPort, DialogueGraphView graphView)
    {
        var targetEdge = graphView.edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            graphView.RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

}