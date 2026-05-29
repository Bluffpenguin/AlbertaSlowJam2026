[CreateAssetMenu(fileName = "CraftingData", menuName = "Crafting/Recipe Database")]
public class CraftingData : ScriptableObject
{
	[SerializeField]
	private Recipe[] _recipes = new Recipe[0];

	public IReadOnlyList<Recipe> Recipes => _recipes;
}
