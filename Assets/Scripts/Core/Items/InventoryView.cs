using UnityEngine.Serialization;

public class InventoryView : MonoBehaviour
{
	[SerializeField]
	[FormerlySerializedAs("_refreshOnStart")]
	protected bool _createOnStart = false;
	[SerializeField]
	protected Inventory _model;
	[SerializeField]
	protected InventoryView _other;
	[SerializeField]
	protected InventorySlot[] _slots;

	public Inventory Source { get => _model; }

	protected virtual void Start()
	{
		if (_createOnStart && _model != null)
			CreateView(_model, _other);
	}

	public virtual void ClearView()
	{
		_model.OnContentsChanged -= UpdateSlotValue;
		for (int i = 0; i < _slots.Length; i++) {
			var slot = _slots[i];
			slot.OnClicked -= Slot_OnClicked;
		}
	}

	public virtual void CreateView(Inventory model, InventoryView otherView)
	{
		Debug.Assert(model.Capacity == _slots.Length);
		_model = model;
		_other = otherView;

		for (int i = 0; i < model.Capacity; i++) {
			var slot = _slots[i];
			slot.SetItemStack(model[i], i);
			slot.OnClicked += this.Slot_OnClicked;
		}
		model.OnContentsChanged += UpdateSlotValue;
	}

	private void Slot_OnClicked(int slotIndex)
	{
		if (_other == null) {
			return;
		}

		var stack = _model[slotIndex];
		if (_other.Source.Add(stack)) {
			Source.RemoveAllAt(slotIndex);
		}
	}

	protected void UpdateSlotValue(int index) => _slots[index].SetItemStack(Source[index], index);
}
