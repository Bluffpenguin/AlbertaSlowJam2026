using System.Linq;

public class Crafter : MonoBehaviour
{
	[SerializeField]
	private CraftingData _database;
	[SerializeField]
	private CraftingResolver _resolver;
	[SerializeField] private List<Ingredient> _contents = new();
	[SerializeField] private Ingredient _output;

	public CraftingData Database => _database;
	public IList<Ingredient> Contents => _contents;
	public Ingredient Output { get => _output; set => _output = value; }

	public void CraftFromContents()
	{
		_output = Craft(_contents);
	}

	const string NO_RESOLUTION_ERROR = "Recipe conflicts detected. Cannot resolve conflicts due to missing resolver.";
	const string NO_RESULT_ERROR = "The ingredients ({0}) does not produce a result. Consider adding a default recipe without any requirements to the database.";
	public Ingredient Craft(IEnumerable<Ingredient> ingredients)
	{
		var query = _database.Recipes.Where(recipe => recipe.CheckRecipe(ingredients)).ToArray();

		Recipe resolvedRecipe;
		switch (query.Length) {
		case 1:
			resolvedRecipe = query[0];
			if (resolvedRecipe == null)
				goto default;
			break;
		case > 1:
			if (_resolver == null) {
				throw new Exception(NO_RESOLUTION_ERROR);
			}
			resolvedRecipe = _resolver.ResolveConflicts(this, query);
			if (resolvedRecipe == null)
				goto default;
			break;
		default:
			var message = string.Join(", ", ingredients.Select(static i => i.name));
			throw new Exception(string.Format(NO_RESULT_ERROR, message));
		}

		int sellValue = resolvedRecipe.CalculateSellValue(ingredients);
		var result = resolvedRecipe.Result;
		Debug.Assert(result != null);
		return result;
	}
}

public abstract class CraftingResolver : MonoBehaviour
{
	public abstract Recipe ResolveConflicts(Crafter crafter, IEnumerable<Recipe> conflicts);
}
