using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip;

    public string Tooltip => _tooltip;

	public bool CanInteract => enabled;

	public void OnInteract()
	{
		Debug.Log(Tooltip);
	}
}
