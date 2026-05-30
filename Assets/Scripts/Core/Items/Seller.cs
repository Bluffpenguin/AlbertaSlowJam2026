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

	private void Awake()
	{
		// TODO: Add listener for on day start to call SetDailySales
		SetDailySales();
	}

	public override bool Add(ItemStack item)
	{
		if (_sales.Contains(item.Data))
		{
			_sellTotal += Mathf.CeilToInt(item.SellValue * _saleMultiplier);
		}
		else
			_sellTotal += item.SellValue;
		

		_sellTotalText.text = '$' + _sellTotal.ToString();
		return base.Add(item);
	}

	public override bool Remove(ItemStack item)
	{
		if (_sales.Contains(item.Data))
		{
			_sellTotal -= Mathf.CeilToInt(item.SellValue * _saleMultiplier);
		}
		else
			_sellTotal -= item.SellValue;

		_sellTotalText.text = '$' + _sellTotal.ToString();
		return base.Remove(item);
	}

	[ContextMenu("Sell Contents")]
	public void SellContents()
	{

		GameManager.Instance.PlayerMoney += _sellTotal;
		_sellTotal = 0;
		_sellTotalText.text = "$0";

		for (int i = 0; i < _inputCount; i++)
		{
			base[i] = ItemStack.Empty;
		}
	}

	void SetDailySales()
	{
		_sales.Clear();
		for (int i = 0; i < _saleCount; i++)
		{
			int rand = Random.Range(0, _acceptedCatalogue.Count);

			if (_sales.Contains(_acceptedCatalogue[rand])) i--;
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
