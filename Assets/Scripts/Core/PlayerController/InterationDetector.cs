using UnityEngine;
using UnityEngine.UI;

public class InterationDetector : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (TryGetComponent(out IInteractable interactable) && Player.Instance.playerInput.Player.Interact.ReadValue<float>()> 0)
		{
			interactable.OnInteract();
		}
	}
}
