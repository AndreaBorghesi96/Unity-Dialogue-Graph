using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueInterface;
    [SerializeField]
    private GameObject optionPrefab;
    [SerializeField]
    private DialogueContainer dialogue;
    [SerializeField]
    private InputActions controls;
    private DialogueNodeData currentNode;
    private TextMeshProUGUI dialogueBoxText;
    private GameObject optionBox;
    private List<GameObject> optionList = new List<GameObject>();
    private bool optionsActive = false;
    private bool ongoing = false;
    public bool Ongoing
    {
        get => ongoing;
    }
    void Start()
    {
        dialogueBoxText = dialogueInterface.transform.Find("Text Box").GetComponentInChildren<TextMeshProUGUI>();
        optionBox = dialogueInterface.transform.Find("Option Box").gameObject;
        dialogueInterface.SetActive(false);
        optionBox.SetActive(false);
    }
    void Update()
    {
        if (ongoing)
        {
            switch (currentNode.NodeType)
            {
                case "SimpleDialogueNode":
                case "ChoiceDialogueNode":
                    break;
                case "BooleanNode":
                    if (ValidateNodeCondition(currentNode))
                        NextNode(currentNode.OutputPortList[0].Guid);
                    else
                        NextNode(currentNode.OutputPortList[1].Guid);
                    break;
                case "EndNode":
                    EndDialogue();
                    break;
                default:
                    NextNode(currentNode.OutputPortList[0].Guid);
                    break;
            }
        }
    }
    void LateUpdate()
    {
        UpdateDialogueBox();
    }
    private void EnableDialogueControls()
    {
        controls.Player.Disable();
        controls.UI.Enable();
    }
    private void DisableDialogueControls()
    {
        controls.Player.Enable();
        controls.UI.Disable();
    }
    void Awake()
    {
        controls = new InputActions();
        controls.UI.Continue.performed += OnContinuePressed;
    }
    public void StartDialogue()
    {
        ongoing = true;
        currentNode = dialogue.GetEntryNode();
        EnableDialogueControls();
    }
    private void EndDialogue()
    {
        ongoing = false;
        currentNode = null;
        DisableDialogueControls();
    }
    private void OnContinuePressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if(currentNode != null && currentNode.NodeType == "SimpleDialogueNode")
        {
            NextNode(currentNode.OutputPortList[0].Guid);
        }
    }
    private void UpdateDialogueBox()
    {
        if (currentNode != null)
        {
            dialogueBoxText.text = currentNode.DialogueText;
            if (currentNode.NodeType == "ChoiceDialogueNode")
            {
                ActivateOptions();
            }
            else
            {
                DeactivateOptions();
            }
        }
        else
        {
            dialogueBoxText.text = "";
            DeactivateOptions();
        }
        dialogueInterface.SetActive(ongoing);
        optionBox.SetActive(optionsActive);
    }
    private void ActivateOptions()
    {
        if(optionsActive) return;
        RectTransform box = optionBox.GetComponent<RectTransform>();
        box.sizeDelta = new Vector2(box.sizeDelta.x, currentNode.OutputPortList.Count() * 50);
        for (var i = 0; i < currentNode.OutputPortList.Count(); i++)
        {
            PortData port = currentNode.OutputPortList[i];
            GameObject option = Instantiate(optionPrefab, optionBox.transform);
            option.GetComponentInChildren<TextMeshProUGUI>().text = port.Text;
            option.GetComponent<OptionButton>().portGuid = port.Guid;
            option.transform.localPosition = new Vector3(0, 20 - 40 * i);
            optionList.Add(option);
        }
        if (optionList.Count() > 0)
        {
            optionList[0].GetComponent<Button>().Select();
        }
        optionsActive = true;
    }
    private void DeactivateOptions()
    {
        if(!optionsActive) return;
        foreach (GameObject option in optionList)
        {
            Destroy(option);
        }
        optionList = new List<GameObject>();
        optionsActive = false;
    }
    private bool ValidateNodeCondition(DialogueNodeData node)
    {
        if (node.NodeType == "ConditionNode")
        {
            return VaildateVariableCondition(node.VarName);
        }
        PortData inputPort = node.InputPortList.Find(p => p.Type == PortType.Logic);
        List<string> conditionNodeGuidList = dialogue.NodeLinks.FindAll(l => l.TargetPortGuid == inputPort.Guid).Select(l => l.BaseNodeGuid).ToList();
        List<DialogueNodeData> conditionNodeList = dialogue.DialogueNodeData.FindAll(n => conditionNodeGuidList.Contains(n.Guid));

        if (conditionNodeList.Count() == 0)
            return true;
        if (node.NodeType == "BooleanNode")
        {
            return ValidateNodeCondition(conditionNodeList[0]);
        }
        if (node.NodeType == "AndNode")
        {
            return conditionNodeList.All(c => ValidateNodeCondition(c));
        }
        if (node.NodeType == "OrNode")
        {
            return conditionNodeList.Any(c => ValidateNodeCondition(c));
        }
        return true;
    }
    private bool VaildateVariableCondition(string varName)
    {
        // Custom logics for validating game variables
        return true;
    }
    public void NextNode(string portGuid)
    {
        if (currentNode.NodeType == "EndNode") return;
        NodeLinkData chosenLink = dialogue.NodeLinks.Find(l => l.BasePortGuid == portGuid);
        currentNode = dialogue.DialogueNodeData.Find(n => n.Guid == chosenLink.TargetNodeGuid);
        return;
    }
}
