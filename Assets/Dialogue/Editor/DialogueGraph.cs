using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private static DialogueGraphView graphView;
    private static string fileName;

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        OpenDialogueGraphWindow("New Dialogue");
    }
    public static void OpenDialogueGraphWindow(string loadedFileName)
    {
        fileName = loadedFileName;
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }
    void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
    }
    void OnDisable()
    {
        rootVisualElement.Add(graphView);
    }
    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap { anchored = true };
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        graphView.Add(miniMap);
    }
    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        toolbar.Add(new Button(() => SaveDialogueTo(fileName)) { text = "Save As..." });

        var fileNameTextField = new TextField();
        fileNameTextField.style.width = 200f;
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        rootVisualElement.Add(toolbar);

    }
    private void SaveDialogueTo(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        saveUtility.SaveGraph(fileName);
    }

    [OnOpenAsset]
    public static bool OpenDialogueAsset(int instanceID, int line)
    {
        UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);

        if (obj is DialogueContainer dialogueContainer)
        {    
            string fileName = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(obj));

            OpenDialogueGraphWindow(fileName);

            var saveUtility = GraphSaveUtility.GetInstance(graphView);
            saveUtility.LoadGraph(fileName);

            return true;
        }
        return false;
    }
}
