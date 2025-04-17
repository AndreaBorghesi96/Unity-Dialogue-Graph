using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InputActions controls;
    private DialogueManager dialogueManager;
    void OnEnable() {
        controls.Player.Enable();
        controls.UI.Disable();
    }
    void OnDisable() {
        controls.Player.Disable();
        controls.UI.Disable();
    }
    void Start()
    {
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
    }
    void Awake()
    {
        controls = new InputActions();
        controls.Player.Interaction.performed += OnInteractionPressed;
    }
    private void OnInteractionPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!dialogueManager.Ongoing)
        {
            dialogueManager.StartDialogue();
        }
    }
}
