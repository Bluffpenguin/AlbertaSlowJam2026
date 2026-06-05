using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }
	public static bool Exists { get => Instance != null; }
	public static PlayerController Controller { get => Instance._controller; }
	public static Inventory Inventory { get => Instance._inventory; }
	public static PersonalInventoryView PersonalView { get => Instance._personalView; }
	public static InputSystem_Actions Input { get => Instance._controller.PlayerInput; }

	[SerializeField] private PlayerController _controller;
	[SerializeField] private Inventory _inventory;
	[SerializeField] private PersonalInventoryView _personalView;

	public void Awake()
	{
		if (Instance != null)
			Debug.LogWarning("Multiple player instances!");
		Instance = this;
		transform.SetParent(null);
	}

	public void OnEnable()
	{
		if (MenuManager.Instance != null) {
			MenuManager.Instance.inGame = true;
		}
	}

	public void OnDisable()
	{
		if (MenuManager.Instance != null) {
			MenuManager.Instance.inGame = false;
		}
	}
}
