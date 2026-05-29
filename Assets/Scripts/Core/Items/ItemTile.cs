using UnityEngine.Tilemaps;

public class ItemTile : MonoBehaviour, IInteractable
{
	private Tilemap _tilemap;
	private Vector3Int _cellPosition;

	[SerializeField] private string _tooltip = "Pick Up";
	[SerializeField] private InventoryItem _item;

	public string Tooltip => _tooltip;
	public bool CanInteract => enabled;

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
		var inventory = Player.Instance.GetComponent<Inventory>();
		Debug.Assert(inventory != null, context: this);
		inventory.Add(stack);
		_tilemap.SetTile(_cellPosition, null);
	}
}
