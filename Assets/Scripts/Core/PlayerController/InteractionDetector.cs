using System.Linq;
using UnityEngine.Tilemaps;

public interface IInteractable
{
	string Tooltip { get; }
	Vector2 TooltipOffset { get; }
	bool CanInteract { get; }
	float InteractTime { get; }

	Renderer MaterialRenderer { get; }
	Material RegularMaterial { get; }
	Material OutlineMaterial { get; }
	

	void OnInteract();
	void ToggleOutline(bool outline);
}

public class InteractionDetector : MonoBehaviour
{
	public bool tryInteract;
	bool progressInteract = false;

	private readonly List<IInteractable> _targets = new();

	float _timeHeld = 0;
	float _targetTime;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.TryGetComponent(out IInteractable interactable)) {
			Debug.Log($"Interaction target is {collider.name}. Tooltip: \"{interactable.Tooltip}\"");
			_targets.Add(interactable);

			if (interactable.CanInteract)
			{
				_timeHeld = 0;
				interactable.ToggleOutline(true);
				_targetTime = interactable.InteractTime;
				TooltipManager.CallTooltip.Invoke(collider.transform.position + (Vector3)interactable.TooltipOffset, interactable.Tooltip);
			}
				
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.TryGetComponent(out IInteractable interactable)) {
			if (interactable.CanInteract && interactable == _targets.LastOrDefault(t => t != null && t.CanInteract))
			{
				_timeHeld = 0;
				TooltipManager.DismisTooltip.Invoke();
			}

			interactable.ToggleOutline(false);
			_targets.Remove(interactable);

		}
	}

	private void Update()
	{
		if (_targets.Count == 0) return;

		if (Player.Input.Player.Interact.WasPressedThisFrame())
		{
			progressInteract = true;
		}
		else if (Player.Input.Player.Interact.WasReleasedThisFrame())
		{
			progressInteract = false;
		}

		if (progressInteract)
		{
			_timeHeld += Time.deltaTime;

			if (_timeHeld >= _targetTime)
			{
				progressInteract = false;
				tryInteract = true;
			}
		}
		else _timeHeld = 0;
	}

	private void FixedUpdate()
	{
		IInteractable target = _targets.LastOrDefault(t => t != null && t.CanInteract);
		if (tryInteract && target != null) {
			tryInteract = false;
			progressInteract = false;
			_targets[0].OnInteract();
		}

		
	}
}
