using UnityEngine.Events;

public class EventInteractable : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip = "Interact";
	[SerializeField] protected UnityEvent _onInteracted = new();

	public event UnityAction OnInteracted {
		add => _onInteracted.AddListener(value);
		remove => _onInteracted.RemoveListener(value);
	}

	public string Tooltip => _tooltip;
	public bool CanInteract => enabled;

	public void OnInteract() => _onInteracted.Invoke();
}
