using UnityEngine;

public class PersonalInventoryView : InventoryView
{
	protected override void Start()
	{
		if (_model != null)
		{
			_model.OnContentsChanged += CreateView;
		}
		base.Start();
	}

	private void OnEnable()
	{
		CreateView(0);
	}

	private void OnDisable()
	{
		ClearView();
	}

	void CreateView(int index)
	{
		ClearView();
		Debug.Assert(_model.Capacity == _slots.Length);

		for (int i = 0; i < _model.Capacity; i++)
		{
			var slot = _slots[i];
			slot.SetItemStack(_model[i], i);
			slot.OnClicked += this.Slot_OnClicked;
		}
		_model.OnContentsChanged += UpdateSlotValue;
	}

	protected override void Slot_OnClicked(int slotIndex)
	{
		if (PlayerItemDrop.Instance != null)
		{
			if (PlayerItemDrop.Instance.DropItem(Player.Controller.transform.position, _model[slotIndex]))
			{
				Source.RemoveAt(slotIndex, 1, out _);
			}
		}

	}
}
