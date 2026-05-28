[CreateAssetMenu(fileName = "Item_", menuName = "Crafting/Inventory Item")]
public class InventoryItem : ScriptableObject
{
	[SerializeField]
	protected string _displayName;
	[SerializeField]
	protected Sprite _sprite;
	[SerializeField]
	[TextArea] protected string _description;
	[SerializeField]
	private int _baseSellValue = 1;

	public string DisplayName {
		get {
			if (string.IsNullOrWhiteSpace(_displayName))
				return name;
			return _displayName;
		}
	}

	public Sprite Sprite => _sprite;
	public string Description => _description;
	public int SellValue { get => _baseSellValue; }
}
