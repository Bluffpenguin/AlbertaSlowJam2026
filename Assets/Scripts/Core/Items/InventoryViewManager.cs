using System.Linq;

public enum InventoryWindow
{
	None = 0,
	Player = 1,
	Storage = 2,
	Scrapper = 3,
	Crafter = 4,
	Seller = 5,
}

public class InventoryViewManager : MonoBehaviour
{
	public static InventoryViewManager Instance { get; private set; }

	[SerializeField] private InventoryWindowData[] _windows;

	private Dictionary<InventoryWindow, (InventoryView, Inventory)> _windowsDict;

	private void Awake()
	{
		if (Instance != null) {
			Destroy(this.gameObject);
			return;
		}

		Instance = this;

		_windowsDict = new(capacity: _windows.Length);
		for (int i = 0; i < _windows.Length; i++) {
			var data = _windows[i];
			_windowsDict.TryAdd(data.window, (data.view, data.model));
		}

		CloseAllOpenViews();
	}

	private void OnEnable()
	{
		Player.Input.Player.Pause.performed += this.Cancel_performed;
	}

	private void OnDisable()
	{
		Player.Input.Player.Pause.performed -= this.Cancel_performed;
	}

	private void Cancel_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) => CloseAllOpenViews();

	public void CloseAllOpenViews()
	{
		foreach ((var view, _) in _windowsDict.Values) {
			view.ClearView();
			view.gameObject.SetActive(false);
		}

		if (MenuManager.Instance != null) {
			MenuManager.Instance.inGame = true;
		}

		if (HUD.Instance != null) {
			HUD.Instance.gameObject.SetActive(true);
		}
	}

	public void CloseAllExcept(params InventoryWindow[] window)
	{
		foreach (var kvp in _windowsDict) {
			if (window.Contains(kvp.Key))
				continue;
			var view = kvp.Value.Item1;
			view.ClearView();
			view.gameObject.SetActive(false);
		}
	}

	public void OpenView(InventoryWindow windowA, InventoryWindow windowB = InventoryWindow.None)
	{
		if (MenuManager.Instance != null) {
			MenuManager.Instance.inGame = false;
		}
		if (HUD.Instance != null) {
			HUD.Instance.gameObject.SetActive(false);
		}

		InventoryView viewA, viewB;
		Inventory modelA, modelB;

		try {
			(viewA, modelA) = GetViewModel(windowA);
			(viewB, modelB) = GetViewModel(windowB);
		}
		catch (KeyNotFoundException ex) {
			Debug.LogWarning(ex.Message);
			return;
		}

		CloseAllExcept(windowA, windowB);
		if (viewA != null) {
			Debug.Assert(modelA != null);
			viewA.gameObject.SetActive(true);
			viewA.CreateView(modelA, viewB);
		}

		if (viewB != null) {
			Debug.Assert(modelB != null);
			viewB.gameObject.SetActive(true);
			viewB.CreateView(modelB, viewA);
		}
	}

	private (InventoryView view, Inventory model) GetViewModel(InventoryWindow window)
	{
		return window switch {
			InventoryWindow.None => (null, null),
			InventoryWindow.Player => (_windowsDict[InventoryWindow.Player].Item1, Player.Inventory),
			_ when _windowsDict.ContainsKey(window) => _windowsDict[window],
			_ => throw new KeyNotFoundException("View does not exist"),
		};
	}

	[Serializable]
	public struct InventoryWindowData
	{
		public InventoryWindow window;
		public InventoryView view;
		public Inventory model;
	}
}
