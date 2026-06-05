using System.Linq;
using TMPro;
using UnityEngine.Events;

public class Seller : Inventory
{
	[SerializeField] 
	List<InventoryItem> _acceptedCatalogue = new List<InventoryItem>();
	[SerializeField]
	List<InventoryItem> _sales = new List<InventoryItem>();
	[SerializeField]
	List<InventorySlot> _saleSlots = new List<InventorySlot>();

	[SerializeField]
	[Min(1)] private int _inputCount = 3;

	[SerializeField]
	private bool _oneTradePerDay = false;

	[SerializeField]
	[Min(1)] private int _saleCount = 3;

	[SerializeField]
	[Min(1)] private float _saleMultiplier = 1.5f;

	[SerializeField]
	TextMeshProUGUI _sellTotalText;

	public int _sellTotal { get; private set; }

	public ItemStack[] Input => base[.._inputCount];


	public override bool Add(ItemStack item)
	{

		int index;
		for (index = 0; index < Capacity; index++)
		{
			if (this[index].IsEmpty() || (_itemsStack && this[index].Data == item.Data))
			{
				break;
			}
		}

		if (index >= Capacity)
		{
			Debug.LogWarning("Cannot Insert item; inventory is full.", this);
			return false;
		}
		else
		{
			this[index] += item;
			if (_sales.Contains(item.Data))
			{
				_sellTotal += Mathf.CeilToInt(item.SellValue * _saleMultiplier * item.Count);
			}
			else
				_sellTotal += item.SellValue * item.Count;


			_sellTotalText.text = '$' + _sellTotal.ToString();
			return true;
		}

	}

	public override bool Remove(ItemStack item)
	{
		Debug.Log("Remove ran");

		if (_sales.Contains(item.Data))
		{
			_sellTotal -= Mathf.CeilToInt(item.SellValue * _saleMultiplier * item.Count);
		}
		else
			_sellTotal -= item.SellValue * item.Count;

		_sellTotalText.text = '$' + _sellTotal.ToString();
		return base.Remove(item);
	}

	public override bool RemoveAllAt(int slot)
	{
		if (_sales.Contains(Input[slot].Data))
		{
			_sellTotal -= Mathf.CeilToInt(Input[slot].SellValue * _saleMultiplier * Input[slot].Count);
		}
		else
			_sellTotal -= Input[slot].SellValue * Input[slot].Count;

		_sellTotalText.text = '$' + _sellTotal.ToString();
		return base.RemoveAllAt(slot);
	}

	public override bool RemoveAt(int slot, int count, out ItemStack removed)
	{
		if (_sales.Contains(Input[slot].Data))
		{
			_sellTotal -= Mathf.CeilToInt(Input[slot].SellValue * _saleMultiplier * count);
		}
		else
			_sellTotal -= Input[slot].SellValue * count;

		_sellTotalText.text = '$' + _sellTotal.ToString();
		return base.RemoveAt(slot, count, out removed);
	}


	[ContextMenu("Sell Contents")]
	public void SellContents()
	{
		GameManager.Instance.PlayerMoney += _sellTotal;
		if (_sellTotal > 0)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.MoneyGained, this.transform.position);
		}
		_sellTotal = 0;
		_sellTotalText.text = "$0";

		for (int i = 0; i < _inputCount; i++)
		{
			base[i] = ItemStack.Empty;
		}
	}

	public void SetDailySales(int saleCount)
	{
		Debug.Log("Setting Sales");
		_saleCount = saleCount;
		_sales.Clear();
		for (int i = 0; i < _saleCount; i++)
		{
			int rand = Random.Range(0, _acceptedCatalogue.Count);

			if (_sales.Contains(_acceptedCatalogue[rand])) i--;
			else if (_saleCount == 1 && _acceptedCatalogue[rand] is Scrap) i--;
			else
			{
				_sales.Add(_acceptedCatalogue[rand]);
			}
		}

		for (int i = 0; i < _saleSlots.Count; i++)
		{
			if (i < _saleCount)
			{
				_saleSlots[i].gameObject.SetActive(true);
				_saleSlots[i].SetItemStack(new ItemStack(_sales[i], 1, Mathf.CeilToInt(_sales[i].SellValue * _saleMultiplier)), i);
			}
			else
				_saleSlots[i].gameObject.SetActive(false);
		}
	}
}
