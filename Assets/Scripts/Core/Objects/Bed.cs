using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip = "Sleep";

	public string Tooltip => _tooltip;
	public bool CanInteract => enabled;

	public void OnInteract()
	{
		GameManager.Instance.MoveToNextDay();
	}
}
