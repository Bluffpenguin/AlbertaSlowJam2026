using UnityEngine;

public class TransitionToShip : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip;

	public string Tooltip => _tooltip;

	public bool CanInteract => enabled;

	public void OnInteract()
	{
		if (TransitionManager.Instance != null) Debug.Log("There is a singleton");
		TransitionManager.Instance.TransitionToShip.Invoke();
	}
}
