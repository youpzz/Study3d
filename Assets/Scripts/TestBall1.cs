using UnityEngine;

public class TestBall : MonoBehaviour, IInteractable
{

    public void Interact()
    {

    }
    public void Hold(PlayerInteraction playerInteraction)
    {

    }
    public string GetInteractionText()
    {
        return "";
    }
    public string GetNameText()
    {
        return "Шар";
    }
}
