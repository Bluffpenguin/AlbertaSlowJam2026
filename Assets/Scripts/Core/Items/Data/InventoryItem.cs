public abstract class InventoryItem : ScriptableObject
{
	[SerializeField]
	protected string _displayName;
	[SerializeField]
	protected Sprite _sprite;
	[SerializeField]
	[TextArea] protected string _description;

	public string DisplayName {
		get {
			if (string.IsNullOrWhiteSpace(_displayName))
				return name;
			return _displayName;
		}
	}

	public Sprite Sprite => _sprite;
	public string Description => _description;
}
