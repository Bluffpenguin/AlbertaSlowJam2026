public class InventoryInteractable : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip = "Open";
	[SerializeField] private InventoryWindow _window1, _window2;

	public string Tooltip => _tooltip;
	public bool CanInteract => enabled;

	public void OnInteract()
	{
		InventoryViewManager.Instance.OpenView(_window1, _window2);
	}
}
