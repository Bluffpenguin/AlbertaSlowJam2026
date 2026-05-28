public class InventoryViewAutoLayout : InventoryView
{
	[Space]
	[SerializeField]
	private bool _refreshOnStart = true;
	[SerializeField]
	private InventorySlot _slotPrefab;
	[SerializeField]
	private Transform _slotParent;

	protected virtual void Start()
	{
		if (_refreshOnStart)
			CreateView(_model, _other);
	}

	public override void ClearView()
	{
		base.ClearView();
		for (int i = 0; i < _slots.Length; i++) {
			Destroy(_slots[i].gameObject);
		}
	}

	public override void CreateView(Inventory inventory, InventoryView otherView)
	{
		if (_slots.Length != inventory.Capacity) {
			Array.Resize(ref _slots, inventory.Capacity);
		}
		for (int i = 0; i < inventory.Capacity; i++) {
			var slot = _slots[i];
			if (slot == null) {
				var clone = Instantiate(_slotPrefab.gameObject, _slotParent);
				clone.SetActive(true);
				slot = clone.GetComponent<InventorySlot>();
				_slots[i] = slot;
			}
		}
		base.CreateView(inventory, otherView);
	}
}
