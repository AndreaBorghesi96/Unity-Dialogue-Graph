using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DialogueManager dialogueManager;
    void Start()
    {
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
    }
    void LateUpdate()
    {
        if(!dialogueManager.Ongoing && Input.GetKeyUp(KeyCode.Space))
        {
            dialogueManager.StartDialogue();
        }
    }
}
