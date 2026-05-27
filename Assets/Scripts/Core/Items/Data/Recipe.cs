using System.Linq;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "NewRecipeData", menuName = "Crafting/Recipe Data")]
public class Recipe : ScriptableObject
{
	[SerializeField]
	[Min(0)] private float _sellValueBonus = 1;

	[SerializeField]
	private RecipeIngredient[] _ingredients;
	[SerializeField]
	private Ingredient _result;

	public float SellValueBonus => _sellValueBonus;
	public Ingredient Result => _result;

	public bool CheckRecipe(IReadOnlyList<Ingredient> ingredients)
	{
		static string Name(Ingredient ingredient) => ingredient.name;

		var requirements = DictionaryPool<string, int>.Get();
		for (int i = 0; i < _ingredients.Length; i++) {
			if (_ingredients[i].Ingredient == null)
				continue;
			requirements.TryAdd(_ingredients[i].Ingredient.name, _ingredients[i].RequiredCount);
		}

		bool valid = ingredients.Count >= requirements.Values.Sum();
		var groups = ingredients.OrderBy(Name).GroupBy(Name);
		foreach (var group in groups) {
			string key = group.Key;
			if (!requirements.ContainsKey(key))
				continue;

			valid &= requirements[key] >= 0;
			int itemCount = group.Count();
			valid &= itemCount >= requirements[key];

			if (!valid)
				break;
		}

		DictionaryPool<string, int>.Release(requirements);
		return valid;
	}

	public int CalculateSellValue(IEnumerable<Ingredient> ingredients)
	{
		float value = ingredients.Sum(static i => i.SellValue) * _sellValueBonus;
		return Mathf.RoundToInt(value);
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
