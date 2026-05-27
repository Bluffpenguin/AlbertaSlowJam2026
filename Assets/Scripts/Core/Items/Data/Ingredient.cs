using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredientData", menuName = "Crafting/Ingredient Data")]
public class Ingredient : InventoryItem
{
	[SerializeField]
	private int _baseSellValue = 1;

	public int BaseSellValue { get => _baseSellValue; }
}
