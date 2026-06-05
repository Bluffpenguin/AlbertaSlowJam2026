using UnityEditor.Graphs;
using UnityEngine.Events;

public class Inventory : MonoBehaviour, IReadOnlyList<ItemStack>
{
	[SerializeField] protected bool _itemsStack = true;
	[SerializeField] protected bool _autoSort = false;
	[SerializeField] protected ItemStack[] _contents = new ItemStack[0];
	[SerializeField] protected UnityEvent<int> _onContentsChanged;

	public bool ItemsStack => _itemsStack;
	public int Capacity => _contents.Length;
	int IReadOnlyCollection<ItemStack>.Count => _contents.Length;

	public event UnityAction<int> OnContentsChanged {
		add => _onContentsChanged.AddListener(value);
		remove => _onContentsChanged.RemoveListener(value);
	}

	public ItemStack this[int index] {
		get => _contents[index];
		set {
			_contents[index] = value;
			if (_autoSort) {
				Sort();
			} else {
				_onContentsChanged.Invoke(index);
			}
		}
	}
	public ItemStack this[Index index] {
		get => this[index.GetOffset(Capacity)];
		set => this[index.GetOffset(Capacity)] = value;
	}
	public ItemStack[] this[Range range] => _contents[range];

	/// <summary>
	/// Tries to add <paramref name="item"/> to the inventory, and returns true if successful.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public virtual bool Add(ItemStack item)
	{
		int index = -1;
		for (int i = 0; i < Capacity; i++) {
			if (this[i].IsEmpty()) {
				if (index < 0)
					index = i;
			} else if (this[i].IsCompatible(item, _itemsStack)) {
				index = i;
				break;
			}
		}

		if (index < 0 || index >= Capacity) {
			Debug.LogWarning("Cannot Insert item; inventory is full.", this);
			return false;
		} else {
			this[index] += item;
			return true;
		}
	}
	/// <summary>
	/// Tries to insert <paramref name="item"/> into <paramref name="slot"/>, and returns true if successful.
	/// </summary>
	/// <param name="slot"></param>
	/// <param name="item"></param>
	/// <returns></returns>
	public virtual bool Insert(int slot, ItemStack item)
	{
		if (item.IsCompatible(this[slot], _itemsStack)) {
			this[slot] += item;
			return true;
		} else {
			return false;
		}
	}
	/// <summary>
	/// Tries to remove <paramref name="item"/> from the inventory and returns true if successful.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public virtual bool Remove(ItemStack item)
	{
		int index;
		for (index = 0; index < Capacity; index++) {
			if (this[index].Data == item.Data) {
				break;
			}
		}

		if (index >= Capacity) {
			return false;
		} else {
			bool allRemoved = this[index].Count >= item.Count;
			this[index] -= item;
			return allRemoved;
		}
	}
	/// <summary>
	/// Tries to remove <paramref name="count"/> items from <paramref name="slot"/>.
	/// </summary>
	/// <param name="slot"></param>
	/// <param name="count"></param>
	/// <param name="removed"></param>
	/// <returns>True if the amount <paramref name="removed"/> matches <paramref name="count"/>.</returns>
	public virtual bool RemoveAt(int slot, int count, out ItemStack removed)
	{
		removed = this[slot] with { Count = Mathf.Min(count, this[slot].Count) };
		this[slot] -= removed;
		return removed.Count == count;
	}

	public virtual bool RemoveRandom(int count, out ItemStack removed)
	{
		for (int i = 0; i < Capacity; i++)
		{
			if (this[i].Count > 0)
			{
				removed = this[i] with { Count = Mathf.Min(count, this[i].Count) };
				this[i] -= removed;
				return removed.Count == count;
			}
		}
		removed = ItemStack.Empty;
		return false;
	}

	public bool IsEmpty()
	{
		foreach (ItemStack item in _contents)
		{
			if (item.Data != null) return false;
		}
		return true;
	}

	public virtual bool RemoveAllAt(int slot) => RemoveAt(slot, this[slot].Count, out _);

	[ContextMenu("Clear Inventory")]
	/// <summary>
	/// Clears the inventory.
	/// </summary>
	public virtual void Clear()
	{
		for (int i = 0; i < Capacity; i++) {
			this[i] = ItemStack.Empty;
			_onContentsChanged.Invoke(i);
		}
	}

	/// <summary>
	/// Sorts the inventory.
	/// </summary>
	[ContextMenu("Sort")]
	public void Sort()
	{
		Array.Sort(_contents);
		for (int i = 0; i < Capacity; i++) {
			_onContentsChanged.Invoke(i);
		}
	}

	public IEnumerator<ItemStack> GetEnumerator()
	{
		for (int i = 0; i < Capacity; i++) {
			yield return this[i];
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Serializable]
public struct ItemStack : IEquatable<ItemStack>, IComparable<ItemStack>
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
	public ItemStack(InventoryItem data, int count) : this(data, count, data.SellValue) { }
	public ItemStack(InventoryItem data, int count, int sellValue)
	{
		_data = data;
		_count = count;
		_sellValue = sellValue;
	}

	public readonly bool IsEmpty() => Data == null || Count <= 0;

	public readonly bool Equals(ItemStack other) => _data == other._data;
	public override readonly bool Equals(object obj) => obj is ItemStack stack && Equals(stack);
	public override readonly int GetHashCode() => HashCode.Combine(_sellValue, _data.name.GetHashCode());

	public override readonly string ToString() => IsEmpty() ? "ItemStack (Empty)" : $"ItemStack({Data.name} x{Count})";

	public readonly int CompareTo(ItemStack other) => (IsEmpty(), other.IsEmpty()) switch {
		(true, true) => 0,
		// Empty stacks are placed last
		(true, false) => +1,
		(false, true) => -1,
		(false, false) => Data.name.CompareTo(other.Data.name),
	};

	/// <summary>
	/// Checks if two item stacks can be combined.
	/// </summary>
	/// <param name="other"></param>
	/// <param name="allowStacking"></param>
	/// <returns></returns>
	public readonly bool IsCompatible(ItemStack other, bool allowStacking = true)
	{
		if (this.IsEmpty() || other.IsEmpty()) {
			return true;
		} else if (allowStacking) {
			return this.Data == other.Data && this.SellValue == other.SellValue;
		} else {
			return false;
		}
	}

	public static ItemStack Combine(ItemStack a, ItemStack b, bool allowStacking = true)
	{
		if (!a.IsCompatible(b, allowStacking)) {
			throw new ArgumentException($"Cannot combine stacks {a} and {b} because they are not compatible.");
		}

		switch (a.IsEmpty(), b.IsEmpty()) {
		case (true, true):
			return Empty;
		case (true, false):
			return b;
		case (false, true):
			return a;
		case (false, false):
			float avg = (a.SellValue + b.SellValue) / 2f;
			return a with {
				Count = a.Count + b.Count,
				SellValue = Mathf.RoundToInt(avg),
			};
		}
	}

	public static ItemStack operator +(ItemStack left, ItemStack right) => Combine(left, right);
	public static ItemStack operator +(ItemStack left, int right)
	{
		left = left with { Count = left.Count + right };
		if (left.IsEmpty())
			return Empty;
		else
			return left;
	}

	public static ItemStack operator -(ItemStack left, ItemStack right) => left + -right.Count;
	public static ItemStack operator -(ItemStack left, int right) => left + -right;
}
