public class Inventory : MonoBehaviour, IReadOnlyList<ItemStack>
{
	[SerializeField] protected bool _itemsStack = true;
	[SerializeField] protected int _maxCapacity = -1;
	[SerializeField] protected List<ItemStack> _contents = new();

	public bool ItemsStack => _itemsStack;
	public int MaxCapacity { get => _maxCapacity; set => _maxCapacity = value; }
	public int Count => _contents.Count;

	public ItemStack this[int index] => _contents[index];

	public virtual void Clear()
	{
		_contents.Clear();
	}

	public virtual bool Add(ItemStack item)
	{
		if (_itemsStack && _contents.Contains(item)) {
			var index = _contents.IndexOf(item);
			Debug.Assert(index >= 0 && _contents[index].Data == item.Data);
			_contents[index] += item;
			return true;
		}

		if (_maxCapacity < 0 || _contents.Count <= _maxCapacity) {
			_contents.Add(item);
			return true;
		} else {
			Debug.LogWarning("Cannot insert item, inventory is full", this);
			return false;
		}
	}

	public virtual bool Remove(ItemStack item)
	{
		int index = -1;
		for (int i = 0; i < _contents.Count; i++) {
			if (_contents[i].Data == item.Data) {
				index = i;
				break;
			}
		}

		if (index < 0)
			return false;

		_contents[index] -= item;
		if (_contents[index].Count <= 0)
			_contents.RemoveAt(index);
		return true;
	}

	public IEnumerator<ItemStack> GetEnumerator() => _contents.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Serializable]
public struct ItemStack : IEquatable<ItemStack>
{
	public static ItemStack Empty { get => new(null, 0, 0); }

	[SerializeField]
	private InventoryItem _data;
	[SerializeField]
	private int _count;
	[SerializeField]
	private int _sellValue;

	public InventoryItem Data { readonly get => _data; set => _data = value; }
	public int Count { readonly get => _count; set => _count = value; }
	public int SellValue { readonly get => _sellValue; set => _sellValue = value; }

	public ItemStack(InventoryItem data) : this(data, 1, data.SellValue) { }
	public ItemStack(InventoryItem data, int count, int sellValue)
	{
		_data = data;
		_count = 0;
		_sellValue = sellValue;
	}

	public readonly bool IsEmpty() => Data == null || Count <= 0;

	public readonly bool Equals(ItemStack other) => _data == other._data && _sellValue == other._sellValue;
	public override readonly bool Equals(object obj) => obj is ItemStack stack && Equals(stack);
	public override readonly int GetHashCode() => HashCode.Combine(_sellValue, _data.name.GetHashCode());

	public static ItemStack operator +(ItemStack left, ItemStack right)
		=> left with { Count = left.Count + right.Count };
	public static ItemStack operator -(ItemStack left, ItemStack right)
		=> left with { Count = left.Count - right.Count };
}
