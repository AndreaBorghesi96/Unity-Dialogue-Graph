using UnityEngine;

public class OptionButton : MonoBehaviour
{
    private DialogueManager dialogueManager;
    public string portGuid;
    void Start()
    {
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
    }
    public void OnOptionSelected()
    {
        dialogueManager.NextNode(portGuid);
    }
}
