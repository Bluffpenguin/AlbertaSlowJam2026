using UnityEngine.Tilemaps;

public class ItemTile : MonoBehaviour, IInteractable
{
	private Tilemap _tilemap;
	private Vector3Int _cellPosition;

	[SerializeField] private string _tooltip = "Pick Up";
	[SerializeField] private Vector2 _tooltipOffset;
	[SerializeField] private InventoryItem _item;

	[SerializeField] private float _interactTime = 2f;
	public float InteractTime => _interactTime;

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

	public void Start()
	{
		_tilemap = GetComponentInParent<Tilemap>();
		_cellPosition = _tilemap.WorldToCell(transform.position);
	}

	void IInteractable.OnInteract() => Pickup();
	public void Pickup()
	{
		if (!Player.Exists)
			return;

		var stack = new ItemStack(_item);
		var inventory = Player.Inventory;
		Debug.Assert(inventory != null, context: this);
		inventory.Add(stack);
		_tilemap.SetTile(_cellPosition, null);
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
