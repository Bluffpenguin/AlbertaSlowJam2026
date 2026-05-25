using UnityEngine;

public interface IInteractable
{
	string tooltip { get; }
	[SerializeField] private string _tooltip;

	public void OnInteract();
}
