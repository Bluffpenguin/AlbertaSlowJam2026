using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
    public string Tooltip { get; }

	public void OnInteract()
	{
		Debug.Log(Tooltip);
	}
}
