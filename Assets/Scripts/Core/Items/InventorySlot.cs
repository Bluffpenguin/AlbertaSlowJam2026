using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour
{
	[SerializeField] private ItemStack _stack;
	[SerializeField] private Button _button;
	[SerializeField] private Image _iconImage;
	[SerializeField] private TextMeshProUGUI _fallbackLabel;
	[SerializeField] private TextMeshProUGUI _countLabel;
	[SerializeField] private TextMeshProUGUI _valueLabel;
	[SerializeField] private TextMeshProUGUI _itemNameLabel;

	private int _slotIndex;

	public event Action<int> OnClicked;

	public void SetItemStack(ItemStack stack, int index)
	{
		_stack = stack;
		_slotIndex = index;

		Debug.Assert(_iconImage != null);
		Debug.Assert(_fallbackLabel != null);
		Debug.Assert(_countLabel != null);
		Debug.Assert(_valueLabel != null);
		Debug.Assert(_itemNameLabel != null);

		if (stack.IsEmpty()) {
			_button.interactable = false;
			_iconImage.enabled = false;
			_fallbackLabel.enabled = false;
			_countLabel.enabled = false;
			_valueLabel.enabled = false;
		} else {
			_button.interactable = true;
			_countLabel.enabled = true;
			_valueLabel.enabled = true;

			_itemNameLabel.text = stack.Data.DisplayName;

			var sprite = stack.Data.Sprite;
			_iconImage.enabled = sprite != null;
			_iconImage.sprite = sprite;

			_fallbackLabel.enabled = sprite == null;
			_fallbackLabel.text = stack.Data.DisplayName;

			_countLabel.text = stack.Count is not (0 or 1) ? $"x{stack.Count}" : string.Empty;
			_valueLabel.text = stack.SellValue != 0 ? $"${stack.SellValue}" : string.Empty;
		}
		_itemNameLabel.gameObject.SetActive(false);
	}

	public void OnSlotClicked()
	{
		Debug.Log($"Slot {_slotIndex} has been clicked");
		OnClicked?.Invoke(_slotIndex);
	}

	private void OnMouseOver()
	{
		_itemNameLabel.gameObject.SetActive(!_stack.IsEmpty());
	}
}
