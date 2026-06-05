using System.Linq;

public class Crafter : Inventory
{
	[SerializeField]
	private CraftingData _database;
	[SerializeField]
	private CraftingResolver _resolver;

	public CraftingData Database => _database;

	public ItemStack[] Input => base[..^1];
	public ItemStack Output { get => base[^1]; set => base[^1] = value; }

	public override bool Add(ItemStack item)
	{
		if (item.Data is Ingredient)
			return base.Add(item);
		else return false;
	}

	public override bool Insert(int slot, ItemStack item)
	{
		if (item.Data is Ingredient)
			return base.Insert(slot, item);
		else return false;
	}

	[ContextMenu("Craft")]
	public void CraftFromContents()
	{
		if (!Output.IsEmpty()) {
			Debug.LogWarning("Cannot craft, output is full");
			return;
		}

		Debug.Log(Input.Length);
		var result = Craft(Recipe.GetIngredients(Input));
		Clear();
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.FinishCraft, this.transform.position);
		Output = result;
	}

	const string NO_RESOLUTION_ERROR = "Recipe conflicts detected. Cannot resolve conflicts due to missing resolver.";
	const string NO_RESULT_ERROR = "The ingredients ({0}) does not produce a result. Consider adding a default recipe without any requirements to the database.";
	public ItemStack Craft(IEnumerable<Ingredient> ingredients)
	{
		if (ingredients?.Count() is null or 0)
			throw new ArgumentException("No items provided.");

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
			Debug.LogWarningFormat(NO_RESULT_ERROR, message);
			return ItemStack.Empty;
		}

		var result = resolvedRecipe.Result;
		Debug.Assert(result != null);
		int sellValue = resolvedRecipe.CalculateSellValue(ingredients);
		return new ItemStack(result, 1, sellValue);
	}
}

public abstract class CraftingResolver : MonoBehaviour
{
	public abstract Recipe ResolveConflicts(Crafter crafter, IEnumerable<Recipe> conflicts);
}
