public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }
	public static bool Exists { get => Instance != null; }

	public InputSystem_Actions playerInput;

	[SerializeField] private float _moveSpeed = 10;
	[SerializeField] private InteractionDetector _detector;

	private Vector2 _moveDir;
	private Rigidbody2D _rb;

	private void Awake()
	{
		Instance = this;
		playerInput = new();
		_rb = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		playerInput.Player.Enable();
		playerInput.Player.Interact.performed += this.Interact_performed;
		playerInput.Player.Interact.canceled += this.Interact_canceled;
	}

	private void OnDisable()
	{
		playerInput.Player.Disable();
		playerInput.Player.Interact.performed -= this.Interact_performed;
		playerInput.Player.Interact.canceled -= this.Interact_canceled;
	}

	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		if (_detector != null) {
			_detector.tryInteract = true;
		}
	}

	private void Interact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		if (_detector != null) {
			_detector.tryInteract = false;
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		_moveDir = playerInput.Player.Move.ReadValue<Vector2>();
		Vector2 displacement = _moveSpeed * Time.fixedDeltaTime * _moveDir;

		_rb.AddForce(displacement, ForceMode2D.Impulse);
	}
}
