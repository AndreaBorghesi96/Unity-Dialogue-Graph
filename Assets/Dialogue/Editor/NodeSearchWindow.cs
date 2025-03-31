using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView graphView;
    private EditorWindow window;
    private Texture2D indentationIcon;

    public void Init(DialogueGraphView dialogueGraphView, EditorWindow editorWindow)
    {
        graphView = dialogueGraphView;
        window = editorWindow;

        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry> {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
            new SearchTreeEntry(new GUIContent("Simple Dialogue", indentationIcon))
            {
                userData = new SimpleDialogueNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("Multiple Choice Dialogue", indentationIcon))
            {
                userData = new ChoiceDialogueNode(), level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Logic Nodes"), 1),
            new SearchTreeEntry(new GUIContent("Boolean Node", indentationIcon))
            {
                userData = new BooleanNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("AND Node", indentationIcon))
            {
                userData = new AndNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("OR Node", indentationIcon))
            {
                userData = new OrNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("Condition Node", indentationIcon))
            {
                userData = new ConditionNode(), level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Other Nodes"), 1),
            new SearchTreeEntry(new GUIContent("Event Node", indentationIcon))
            {
                userData = new EventNode(), level = 2
            },
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
            context.screenMousePosition - window.position.position);

        var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        return graphView.CreateNode((BaseNode) searchTreeEntry.userData, localMousePosition);
    }
}