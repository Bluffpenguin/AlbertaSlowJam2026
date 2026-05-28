public class InventoryView : MonoBehaviour
{
	[SerializeField]
	private bool _refreshOnStart = true;
	[SerializeField]
	private InventorySlot _slotPrefab;
	[SerializeField]
	private Transform _slotParent;
	[SerializeField]
	private Inventory _target;
	[SerializeField]
	private InventoryView _other;

	[Space]
	[SerializeField]
	private InventorySlot[] _itemSlots = new InventorySlot[0];

	public void Start()
	{
		if (_refreshOnStart)
			Refresh();
	}

	[ContextMenu("Refresh View")]
	public void Refresh()
	{
		var target = _target;
		var other = _other;
		ResetView();
		CreateView(target, other);
	}

	public void ClearView()
	{
		foreach (Transform child in _slotParent) {
			if (child.TryGetComponent(out InventorySlot component)) {
				component.OnClicked -= TransferSlot;
				Destroy(component.gameObject);
			}
		}
		_target = null;
		_other = null;
	}

	public void ResetView()
	{
		for (int i = 0; i < _itemSlots.Length; i++) {
			InventorySlot slot = _itemSlots[i];
			slot.OnClicked -= TransferSlot;
		}
		_target.OnContentsChanged -= UpdateSlot;
	}

	public void CreateView(Inventory inventory, InventoryView otherView = null)
	{
		_target = inventory;
		_other = otherView;

		if (_itemSlots.Length != inventory.Capacity) {
			Array.Resize(ref _itemSlots, inventory.Capacity);
		}

		for (int i = 0; i < inventory.Capacity; i++) {
			InventorySlot slotComponent;
			if (_itemSlots[i] == null) {
				var slotObj = Instantiate(_slotPrefab.gameObject, _slotParent);
				slotComponent = slotObj.GetComponent<InventorySlot>();
				_itemSlots[i] = slotComponent;
			} else {
				slotComponent = _itemSlots[i];
			}

			slotComponent.SetItemStack(inventory[i], i);
			slotComponent.OnClicked += TransferSlot;
		}
		_target.OnContentsChanged += UpdateSlot;
	}

	public void TransferSlot(int index)
	{
		Debug.Log($"Attempting to move slot {index}");
		if (_other == null)
			return;

		var stack = _target[index];
		if (_other._target.Add(stack)) {
			_target.RemoveAllAt(index);
		}
	}

	public void UpdateSlot(int index)
	{
		_itemSlots[index].SetItemStack(_target[index], index);
	}
}
