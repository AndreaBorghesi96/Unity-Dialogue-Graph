using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueGraphView targetGraphView;
    private DialogueContainer containerCache;
    private List<Edge> Edges => targetGraphView.edges.ToList();
    private List<BaseNode> Nodes => targetGraphView.nodes.ToList().Cast<BaseNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView graphView)
    {
        return new GraphSaveUtility
        {
            targetGraphView = graphView
        };
    }
    public void SaveGraph(string fileName)
    {
        containerCache = Resources.Load<DialogueContainer>($"Dialogues/{fileName}");
        if (containerCache != null && !EditorUtility.DisplayDialog("Confirm overwrite",
                    "A dialogue with this name already exists, do you want to overwrite it?", "Confirm", "Cancel"))
            return;

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (var i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as BaseNode;
            var inputNode = connectedPorts[i].input.node as BaseNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BasePortGuid = ((PortData)connectedPorts[i].output.userData).Guid,
                TargetPortGuid = ((PortData)connectedPorts[i].input.userData).Guid,
            });
        }

        foreach (var node in Nodes)
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                Guid = node.Guid,
                NodeType = node.GetType().ToString(),
                DialogueText = node is DialogueNode ? (node as DialogueNode).DialogueText : null,
                Position = node.GetPosition().position,
                OutputPortList = node.Query<Port>().ToList().Where(port => port.direction == Direction.Output).Select(port => (PortData)port.userData).ToList(),
                InputPortList = node.Query<Port>().ToList().Where(port => port.direction == Direction.Input).Select(port => (PortData)port.userData).ToList()
            });
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
        }

        AssetDatabase.DeleteAsset($"Assets/Resources/Dialogues/{fileName}.asset");
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Dialogues/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }
    public void LoadGraph(string fileName)
    {
        containerCache = Resources.Load<DialogueContainer>($"Dialogues/{fileName}");
        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File not found", "Dialogue graph file does not exist", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }
    public void ClearGraph()
    {
        foreach (var node in Nodes)
        {
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => targetGraphView.RemoveElement(edge));
            targetGraphView.RemoveElement(node);
        }
    }
    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.DialogueNodeData)
        {
            switch (nodeData.NodeType)
            {
                case "EntryNode":
                    targetGraphView.AddElement(targetGraphView.CreateEntryNode(nodeData));
                    break;
                case "EndNode":
                    targetGraphView.AddElement(targetGraphView.CreateEndNode(nodeData));
                    break;
                case "SimpleDialogueNode":
                    targetGraphView.AddElement(targetGraphView.CreateSimpleNode(nodeData.Position, nodeData));
                    break;
                case "ChoiceDialogueNode":
                    targetGraphView.AddElement(targetGraphView.CreateChoiceNode(nodeData.Position, nodeData));
                    break;
                case "BooleanNode":
                    targetGraphView.AddElement(targetGraphView.CreateBooleanNode(nodeData.Position, nodeData));
                    break;
                case "AndNode":
                    targetGraphView.AddElement(targetGraphView.CreateAndNode(nodeData.Position, nodeData));
                    break;
                case "OrNode":
                    targetGraphView.AddElement(targetGraphView.CreateOrNode(nodeData.Position, nodeData));
                    break;
                case "ConditionNode":
                    targetGraphView.AddElement(targetGraphView.CreateConditionNode(nodeData.Position, nodeData));
                    break;
                default:
                    continue;
            }
        }
    }
    private void ConnectNodes()
    {
        foreach (var link in containerCache.NodeLinks)
        {
            var basePort = FindPortByGuid(link.BasePortGuid);
            var targetPort = FindPortByGuid(link.TargetPortGuid);
            if (basePort != null && targetPort != null)
            {
                LinkNodes(basePort, targetPort);
            }
        }
    }
    private Port FindPortByGuid(string portGuid)
    {
        foreach (var node in Nodes)
        {
            var nodePorts = node.Query<Port>().ToList();
            foreach (var port in nodePorts)
            {
                if (((PortData)port.userData).Guid == portGuid)
                {
                    return port;
                }
            }
        }
        return null;
    }
    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        targetGraphView.Add(tempEdge);
    }
}