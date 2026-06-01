public class InventoryInteractable : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip = "Open";
	[SerializeField] private Vector2 _tooltipOffset;
	[SerializeField] private InventoryWindow _window1, _window2;

	[Header("Outline (Optional)")]
	[SerializeField] private Renderer _renderer;
	[SerializeField] private Material _regularMaterial;
	[SerializeField] private Material _outlineMaterial;

	public string Tooltip => _tooltip;
	public bool CanInteract => enabled;

	public Vector2 TooltipOffset => _tooltipOffset;

	public Renderer MaterialRenderer => _renderer;
	public Material RegularMaterial => _regularMaterial;

	public Material OutlineMaterial => _outlineMaterial;

	public void OnInteract()
	{
		InventoryViewManager.Instance.OpenView(_window1, _window2);
	}

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
