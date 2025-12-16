
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// lets the player look at things and press a button to use them
public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 2f; // how close you need to be

    public GameObject interactionUI; // the little text that pops up
    public TextMeshProUGUI interactionText;

    // check for interactable objects constantly
    private void Update()
    {
        InteractionRay();
    }

    // shoots an invisible laser from the player's eyes
    void InteractionRay()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool hitSomething = false;

        // if the laser hits something close enough
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            // if it's something we can actually use
            if (interactable != null)
            {
                hitSomething = true;
                interactionText.text = interactable.GetDescription();

                // if player presses 'E', do the action
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    interactable.Interact();
                }
            }
        }

        // show or hide the text depending on if we are looking at something
        interactionUI.SetActive(hitSomething);
    }
}