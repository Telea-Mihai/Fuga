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

    private IInteractable current;
    void Update()
    {
        if(Input.GetKeyDown(interactKey) && current != null)
            current.Interact(this);
        
        checkForInteractable();
    }

    void checkForInteractable()
    {
        if (Physics.Raycast(pov.position, pov.forward, out RaycastHit hit, range, layerMask, QueryTriggerInteraction.Collide))
            current = hit.transform.GetComponent<IInteractable>();
        else
            current = null;
    }
}
