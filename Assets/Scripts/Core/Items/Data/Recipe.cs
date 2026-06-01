using System.Linq;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Recipe_", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
	[SerializeField]
	[Min(0)] private float _sellValueBonus = 1;

	[SerializeField]
	private RecipeIngredient[] _ingredients;
	[SerializeField]
	private InventoryItem _result;

	public float SellValueBonus => _sellValueBonus;
	public InventoryItem Result => _result;

	public bool CheckRecipe(IEnumerable<Ingredient> input)
	{
		var requirements = DictionaryPool<string, int>.Get();
		for (int i = 0; i < _ingredients.Length; i++) {
			if (_ingredients[i].Ingredient == null)
				continue;
			requirements.TryAdd(_ingredients[i].Ingredient.name, _ingredients[i].RequiredCount);
		}

		var ingredients = DictionaryPool<string, int>.Get();
		foreach (var item in input) {
			if (item == null)
				continue;

			if (!ingredients.TryAdd(item.name, 1)) {
				ingredients[item.name] += 1;
			}
		}

		bool valid = true;
		foreach (var key in requirements.Keys) {
			valid &= ingredients.TryGetValue(key, out var inputCount);
			if (!valid)
				break;
			valid &= inputCount >= requirements[key];
			if (!valid)
				break;
		}

		DictionaryPool<string, int>.Release(requirements);
		DictionaryPool<string, int>.Release(ingredients);
		return valid;
	}

	public int CalculateSellValue(IEnumerable<Ingredient> ingredients)
	{
		float value = ingredients.Sum(static i => i == null ? 0 : i.SellValue) * _sellValueBonus;
		return Mathf.RoundToInt(value);
	}

	public static Ingredient[] GetIngredients(IEnumerable<ItemStack> input)
	{
		return input.Where(static i => !i.IsEmpty()).SelectMany(Expand).ToArray();

		static IEnumerable<Ingredient> Expand(ItemStack item)
		{
			if (item.Data is not Ingredient)
				yield break;

			for (int i = 0; i < item.Count; i++) {
				yield return item.Data as Ingredient;
			}
		}
	}
}

[Serializable]
public struct RecipeIngredient
{
	[SerializeField]
	private Ingredient _ingredient;
	[SerializeField]
	private int _requiredCount;

	public Ingredient Ingredient { readonly get => _ingredient; set => _ingredient = value; }
	public int RequiredCount { readonly get => _requiredCount; set => _requiredCount = value; }
}
