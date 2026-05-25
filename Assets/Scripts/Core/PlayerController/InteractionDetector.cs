using System.Linq;

public interface IInteractable
{
	string Tooltip { get; }
	bool CanInteract { get; }

	void OnInteract();
}

public class InteractionDetector : MonoBehaviour
{
	public bool tryInteract;

	private readonly List<IInteractable> _targets = new();

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (TryGetComponent(out IInteractable interactable)) {
			_targets.Add(interactable);
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.TryGetComponent(out IInteractable interactable)) {
			_targets.Remove(interactable);
		}
	}

	private void FixedUpdate()
	{
		IInteractable target = _targets.LastOrDefault(t => t != null && t.CanInteract);
		if (tryInteract && target != null) {
			tryInteract = false;
			_targets[0].OnInteract();
		}
	}
}
