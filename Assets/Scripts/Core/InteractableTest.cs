using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
    public string tooltip { get; }

	public void OnInteract()
	{
		Debug.Log(tooltip);
	}
}
