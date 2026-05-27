using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
	[SerializeField] private InventoryItem _data;
	[SerializeField] private Button _button;
	[SerializeField] private Image _iconImage;
	[SerializeField] private Text _fallbackLabel;
	[SerializeField] private Text _countLabel;
	[SerializeField] private Text _valueLabel;

	private int _slotIndex;

	public event Action<int> OnClicked;

	public void SetItemStack(ItemStack stack, int index)
	{
		_data = stack.Data;
		_slotIndex = index;

		Debug.Assert(_iconImage != null);
		Debug.Assert(_fallbackLabel != null);
		Debug.Assert(_countLabel != null);
		Debug.Assert(_valueLabel != null);

		var sprite = stack.Data.Sprite;
		_iconImage.enabled = sprite != null;
		_iconImage.sprite = sprite;

		_fallbackLabel.enabled = sprite == null;
		_fallbackLabel.text = stack.Data.name;

		_countLabel.text = stack.Count is not (0 or 1) ? $"x{stack.Count}" : string.Empty;
		_valueLabel.text = stack.SellValue != 0 ? $"${stack.SellValue}" : string.Empty;
	}

	public void OnSlotClicked()
	{
		OnClicked?.Invoke(_slotIndex);
	}
}
