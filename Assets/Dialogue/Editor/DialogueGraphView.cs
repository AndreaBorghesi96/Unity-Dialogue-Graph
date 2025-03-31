using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    private NodeSearchWindow searchWindow;
    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(CreateEntryNode());
        AddElement(CreateEndNode());
        AddSearchWindow(editorWindow);
    }
    private void AddSearchWindow(EditorWindow editorWindow)
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(this, editorWindow);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }
    public EntryNode CreateEntryNode(DialogueNodeData nodeData = null)
    {
        var node = new EntryNode
        {
            title = "START",
            Guid = nodeData != null ? nodeData.Guid :  Guid.NewGuid().ToString()
        };

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddFlowExitPort(node, nodeData != null ? nodeData.GetFlowExitPort() : null);
        node.RefreshPorts();
        node.RefreshExpandedState();

        node.SetPosition(new Rect(50, 200, 100, 150));
        return node;
    }
    public EndNode CreateEndNode(DialogueNodeData nodeData = null)
    {
        var node = new EndNode
        {
            title = "END",
            Guid = nodeData != null ? nodeData.Guid : Guid.NewGuid().ToString()
        };
        node.capabilities &= ~Capabilities.Deletable;

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddFlowEntryPort(node, nodeData != null ? nodeData.GetFlowEntryPort() : null);
        node.RefreshPorts();
        node.RefreshExpandedState();

        node.SetPosition(nodeData != null ? new Rect(nodeData.Position, defaultNodeSize) : new Rect(750, 200, 100, 150));
        return node;
    }
    public bool CreateNode(BaseNode nodeType, Vector2 position)
    {
        switch (nodeType)
        {
            case SimpleDialogueNode:
                AddElement(CreateSimpleNode(position));
                return true;
            case ChoiceDialogueNode:
                AddElement(CreateChoiceNode(position));
                return true;
            case BooleanNode:
                AddElement(CreateBooleanNode(position));
                return true;
            case AndNode:
                AddElement(CreateAndNode(position));
                return true;
            case OrNode:
                AddElement(CreateOrNode(position));
                return true;
            case ConditionNode:
                AddElement(CreateConditionNode(position));
                return true;
            case EventNode:
                AddElement(CreateEventNode(position));
                return true;
            default:
                return false;
        }
    }
    public BooleanNode CreateBooleanNode(Vector2 position, DialogueNodeData nodeData = null)
    {
        var node = new BooleanNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddFlowEntryPort(node, nodeData != null ? nodeData.GetFlowEntryPort() : null);
        NodeCreateUtility.AddLogicEntryPort(node, Port.Capacity.Single, nodeData != null ? nodeData.GetLogicEntryPort() : null);
        NodeCreateUtility.AddBooleanExitPorts(node, nodeData != null ? nodeData.GetTrueExitPort() : null, nodeData != null ? nodeData.GetFalseExitPort() : null);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public AndNode CreateAndNode(Vector2 position, DialogueNodeData nodeData = null)
    {
        var node = new AndNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddLogicExitPort(node, nodeData != null ? nodeData.GetLogicExitPort() : null);
        NodeCreateUtility.AddLogicEntryPort(node, Port.Capacity.Multi, nodeData != null ? nodeData.GetLogicEntryPort() : null);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public OrNode CreateOrNode(Vector2 position, DialogueNodeData nodeData = null)
    {
        var node = new OrNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddLogicExitPort(node, nodeData != null ? nodeData.GetLogicExitPort() : null);
        NodeCreateUtility.AddLogicEntryPort(node, Port.Capacity.Multi, nodeData != null ? nodeData.GetLogicEntryPort() : null);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public ConditionNode CreateConditionNode(Vector2 position, DialogueNodeData nodeData = null)
    {
        var node = new ConditionNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddLogicExitPort(node, nodeData != null ? nodeData.GetLogicExitPort() : null);
        node.RefreshPorts();
        node.RefreshExpandedState();
        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public SimpleDialogueNode CreateSimpleNode(Vector2 position, DialogueNodeData nodeData = null)
    {
        var node = new SimpleDialogueNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddFlowEntryPort(node, nodeData != null ? nodeData.GetFlowEntryPort() : null);
        NodeCreateUtility.AddFlowExitPort(node, nodeData != null ? nodeData.GetFlowExitPort() : null);
        NodeCreateUtility.AddTextBox(node);
        node.RefreshPorts();
        node.RefreshExpandedState();

        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public ChoiceDialogueNode CreateChoiceNode(Vector2 position, DialogueNodeData nodeData = null, List<NodeLinkData> nodeLinkData = null)
    {
        var node = new ChoiceDialogueNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddFlowEntryPort(node, nodeData != null ? nodeData.GetFlowEntryPort() : null);
        NodeCreateUtility.AddCustomFlowExitPorts(node, this, nodeData != null ? nodeData.GetFlowExitPorts() : null);
        NodeCreateUtility.AddTextBox(node);

        var button = new Button(() =>
        {
            NodeCreateUtility.AddCustomFlowExitPort(node, this);
        });
        button.text = "New choice";
        node.titleContainer.Add(button);

        node.RefreshPorts();
        node.RefreshExpandedState();

        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public EventNode CreateEventNode(Vector2 position, DialogueNodeData nodeData = null)
    {
        var node = new EventNode(nodeData);

        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        NodeCreateUtility.AddFlowEntryPort(node, nodeData != null ? nodeData.GetFlowEntryPort() : null);
        NodeCreateUtility.AddFlowExitPort(node, nodeData != null ? nodeData.GetFlowExitPort() : null);
        
        var inputField = new ObjectField();
        inputField.objectType = typeof(ScriptableObject);
        inputField.allowSceneObjects = false;
        inputField.RegisterValueChangedCallback(evt =>
        {
            Debug.Log("Selected ScriptableObject: " + evt.newValue);
        });

        node.contentContainer.Add(inputField);

        node.RefreshPorts();
        node.RefreshExpandedState();

        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction &&
                ((PortData)startPort.userData).Type == ((PortData)port.userData).Type)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
}
