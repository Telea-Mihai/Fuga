using System;
using UnityEngine;

public interface IInteractable
{
    string GetInteractionPrompt();

    void Interact(PlayerInteraction interaction);
}

public class PlayerInteraction : MonoBehaviour
{
    public float range;
    public LayerMask layerMask;
    public KeyCode interactKey;
    public Transform pov;
    
    private Itempickup itempickup;

    private IInteractable current;
    [HideInInspector] public bool hovering;
    [HideInInspector] public string message;
    public bool canInteract;
    
    private DialogManager dialogManager;

    private void Start()
    {
        itempickup = GetComponent<Itempickup>();
        dialogManager = DialogManager.Instance;
    }

    void Update()
    {
        if (dialogManager.inDialog)
        {
            if(Input.GetKeyDown(interactKey))
                dialogManager.ShowNextLine();
            return;
        }
        if (Input.GetKeyDown(interactKey) && current != null && canInteract)
        {
            current.Interact(this);
            if(itempickup)
                itempickup.DropObject();
        }
        if(canInteract)
            checkForInteractable();
    }

    void checkForInteractable()
    {
        if (Physics.Raycast(pov.position, pov.forward, out RaycastHit hit, range, layerMask, QueryTriggerInteraction.Collide))
        {
            current = hit.collider.transform.GetComponent<IInteractable>();
            Debug.Log(hit.transform.name);
            if(current==null) return;
            message = current.GetInteractionPrompt();
            hovering = true;
        }
        else
        {
            hovering = false;
            current = null;
            message = "";
        }
    }
    
    
}
