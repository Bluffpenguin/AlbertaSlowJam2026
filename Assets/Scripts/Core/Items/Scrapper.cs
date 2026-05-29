using System.Linq;

public class Scrapper : Inventory
{
	[SerializeField]
	[Min(1)] private int _inputCount = 3;

	public ItemStack[] Input => base[.._inputCount];
	public ItemStack[] Output => base[_inputCount..];

	public override bool Add(ItemStack item)
	{
		if (item.Data is not Scrap)
			return false;
		else
			return base.Add(item);
	}

	[ContextMenu("Scrap Contents")]
	public void ScrapContents()
	{
		if (Output.All(i => i.IsEmpty()) is false) {
			Debug.LogWarning("Cannot scrap, output is full");
			return;
		}

		foreach (var item in Input) {
			if (item.Data is not Scrap scrap)
				continue;

			var results = scrap.GenerateResults(item.Count).Select(kvp => new ItemStack(kvp.Key, kvp.Value));
			Debug.Log($"Scrapped {scrap.name} into: {string.Join(", ", results)}");

			foreach (var result in results) {
				for (int i = _inputCount; i < Capacity; i++) {
					if (base[i].IsEmpty() || base[i].Equals(result)) {
						base[i] += result;
						break;
					}
				}
			}
		}

		for (int i = 0; i < _inputCount; i++) {
			base[i] = ItemStack.Empty;
		}
	}
}
