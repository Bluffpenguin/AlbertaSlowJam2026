public class InventoryView : MonoBehaviour
{
	[SerializeField]
	private InventorySlot _slotPrefab;
	[SerializeField]
	private Transform _slotParent;
	[SerializeField]
	private Inventory _target;
	[SerializeField]
	private InventoryView _other;

	[ContextMenu("Refresh View")]
	public void Refresh()
	{
		var target = _target;
		var other = _other;
		ClearView();
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

	public void CreateView(Inventory inventory, InventoryView otherView = null)
	{
		ClearView();
		_target = inventory;
		_other = otherView;

		for (int i = 0; i < ((IReadOnlyCollection<ItemStack>)inventory).Count; i++) {
			ItemStack item = inventory[i];
			var slotObj = Instantiate(_slotPrefab.gameObject, _slotParent);
			var slotComponent = slotObj.GetComponent<InventorySlot>();
			slotComponent.SetItemStack(item, i);
			slotComponent.OnClicked += TransferSlot;
		}
	}

	public void TransferSlot(int index)
	{
		if (_other == null)
			return;

		var stack = _target[index];
		if (_other._target.Add(stack)) {
			_target.RemoveAllAt(index);
		}
	}
}
