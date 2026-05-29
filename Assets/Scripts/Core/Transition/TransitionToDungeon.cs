using UnityEngine;

public class TransitionToDungeon : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip;
	public string Tooltip => _tooltip;

	public bool CanInteract => enabled;

	public void OnInteract()
	{
		TransitionManager.Instance.TransitionToDungeon.Invoke();
	}
}
