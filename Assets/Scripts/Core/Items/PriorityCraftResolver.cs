using System.Linq;

public class PriorityCraftResolver : CraftingResolver
{
	public override Recipe ResolveConflicts(Crafter crafter, IEnumerable<Recipe> conflicts)
	{
		return conflicts.OrderByDescending(static recipe => recipe.SellValueBonus).FirstOrDefault();
	}
}
