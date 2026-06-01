using UnityEngine.Events;

public class EventInteractable : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip = "Interact";
	[SerializeField] private Vector2 _tooltipOffset;
	[SerializeField] protected UnityEvent _onInteracted = new();

	[Header("Outline (Optional)")]
	[SerializeField] private Renderer _renderer;
	[SerializeField] private Material _regularMaterial;
	[SerializeField] private Material _outlineMaterial;

	public event UnityAction OnInteracted {
		add => _onInteracted.AddListener(value);
		remove => _onInteracted.RemoveListener(value);
	}

	public string Tooltip => _tooltip;
	public bool CanInteract => enabled;

	public Vector2 TooltipOffset => _tooltipOffset;

	public Renderer MaterialRenderer => _renderer;
	public Material RegularMaterial => _regularMaterial;

	public Material OutlineMaterial => _outlineMaterial;

	public void OnInteract() => _onInteracted.Invoke();

	public void ToggleOutline(bool outline)
	{
		if (_renderer == null) return;


		if (outline)
		{
			_renderer.material = _outlineMaterial;
		}
		else
			_renderer.material = _regularMaterial;
	}
}
