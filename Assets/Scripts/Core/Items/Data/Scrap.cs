[CreateAssetMenu(fileName = "NewScrapData", menuName = "Crafting/Scrap Data")]
public class Scrap : InventoryItem
{
	[SerializeField]
	private ScrapResult[] _results = Array.Empty<ScrapResult>();

	public Dictionary<Ingredient, int> GenerateResults()
	{
		var result = new Dictionary<Ingredient, int>();

		foreach (var item in _results) {
			int count = 0;
			for (int i = 0; i < item.Count; i++) {
				if (Random.value < item.DropRate)
					count++;
			}

			if (result.ContainsKey(item.Ingredient)) {
				result[item.Ingredient] += count;
			} else {
				result.Add(item.Ingredient, count);
			}
		}

		return result;
	}
}

[Serializable]
public struct ScrapResult
{
	[SerializeField]
	private Ingredient _ingredient;
	[SerializeField]
	[Min(0)] private int _count = 1;
	[SerializeField]
	[Range(0, 1)] private float _dropRate = 1.0f;

	public Ingredient Ingredient { readonly get => _ingredient; set => _ingredient = value; }
	public int Count { readonly get => _count; set => _count = value; }
	public float DropRate { readonly get => _dropRate; set => _dropRate = value; }

	public ScrapResult() : this(null, 1, 1f) { }
	public ScrapResult(Ingredient ingredient, int count, float dropRate)
	{
		_ingredient = ingredient;
		_count = count;
		_dropRate = dropRate;
	}
}
