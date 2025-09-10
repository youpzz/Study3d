using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image holdProgressBar;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private TMP_Text itemNameText;

    [Header("Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask resourcesLayer;
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private float maxHold = 1f;

    [Header("Effects")]
    [SerializeField] private GameObject hitParticle;

    private IInteractable currentInteractable;
    private float holdProgress;

    private Outline outline;

    // --- Unity Methods ---
    private void Update()
    {
        HandleUI();
        DetectInteractable();
        HandleInteractionInput();
    }

    // --- Core Logic ---
    private void DetectInteractable()
    {
        Ray ray = GetCenterRay();
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            if (IsVisible(hit))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    if (interactable != currentInteractable)
                    {
                        outline = hit.collider.gameObject.GetComponent<Outline>();
                        if (outline) outline.enabled = true;

                        ResetCurrentInteractable();
                        currentInteractable = interactable;
                        ShowInteractionPrompt(interactable.GetNameText(), interactable.GetInteractionText());
                    }
                    return;
                }
            }
        }

        ResetCurrentInteractable();
    }

    private void HandleInteractionInput()
    {
        if (currentInteractable == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact(currentInteractable);
        }

        if (Input.GetKey(KeyCode.E))
        {
            // Hold(currentInteractable);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            holdProgress = 0;
        }
    }


    // --- Interaction ---
    private void Interact(IInteractable interactable)
    {
        interactable.Interact();
        ResetCurrentInteractable();
    }

    private void Hold(IInteractable interactable)
    {


    }

    private void ResetCurrentInteractable()
    {
        if (currentInteractable == null) return;
        if (outline) outline.enabled = false;
        holdProgress = 0;
        currentInteractable = null;
        HideInteractionPrompt();
    }

    // --- UI ---
    private void HandleUI()
    {
        if (!holdProgressBar) return;

        holdProgressBar.gameObject.SetActive(holdProgress > 0);
        holdProgressBar.fillAmount = holdProgress / maxHold;
    }

    private void ShowInteractionPrompt(string itemName, string interactName)
    {
        itemNameText.text = itemName;
        interactionText.text = interactName;

        itemNameText.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(true);
    }

    private void HideInteractionPrompt()
    {
        itemNameText.gameObject.SetActive(false);
        interactionText.gameObject.SetActive(false);
    }

    // --- Helpers ---
    private Ray GetCenterRay() =>
        playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

    private bool IsVisible(RaycastHit hit)
    {
        Vector3 directionToTarget = (hit.point - playerCamera.transform.position).normalized;
        float distanceToTarget = Vector3.Distance(playerCamera.transform.position, hit.point);

        return !Physics.Raycast(playerCamera.transform.position, directionToTarget, distanceToTarget, obstacleLayer);
    }
}

// --- Interface ---
public interface IInteractable
{
    void Interact();
    void Hold(PlayerInteraction playerInteraction);
    string GetInteractionText();
    string GetNameText();
}
