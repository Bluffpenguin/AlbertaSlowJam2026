public class PlayerController : MonoBehaviour
{
	public InputSystem_Actions PlayerInput { get => _playerInput; }
	public Rigidbody2D Rigidbody { get => _rb; }

	private InputSystem_Actions _playerInput;

	[SerializeField] private bool _disablePause;
	[SerializeField] private InteractionDetector _detector;
	[SerializeField] private float _moveSpeed = 10;
	[Space]
	[SerializeField] private float _dashSpeed = 200;
	[SerializeField] private float _dashCooldown = 5f;

	private Rigidbody2D _rb;
	private Vector2 _moveDir;
	private Vector2 _dashDirection;
	private float _dashCooldownTimer;

	private bool _canMove = true;
	Coroutine _currentStun = null;

	private void Awake()
	{
		_playerInput = new();
		_rb = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		_playerInput.Player.Enable();
		_playerInput.Player.Interact.performed += this.Interact_performed;
		_playerInput.Player.Interact.canceled += this.Interact_canceled;
		_playerInput.Player.Pause.performed += this.Pause_performed;
	}

	private void OnDisable()
	{
		_playerInput.Player.Disable();
		_playerInput.Player.Interact.performed -= this.Interact_performed;
		_playerInput.Player.Interact.canceled -= this.Interact_canceled;
		_playerInput.Player.Pause.performed -= this.Pause_performed;
	}

	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		if (_detector != null) {
			//_detector.tryInteract = true;
		}
	}

	private void Interact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		if (_detector != null) {
			//_detector.tryInteract = false;
		}
	}

	private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		if (_disablePause)
			return;
		MenuManager.Instance.Pause_and_Unpause();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!_canMove) return;

		_moveDir = _playerInput.Player.Move.ReadValue<Vector2>();

		Vector2 displacement = _moveSpeed * Time.fixedDeltaTime * _moveDir;
		_rb.AddForce(displacement, ForceMode2D.Impulse);
		if (_rb.linearVelocity != Vector2.zero) {
			_dashDirection = _rb.linearVelocity.normalized;
		}

		_dashCooldownTimer -= Time.fixedDeltaTime;
		if (_playerInput.Player.Dash.IsPressed() && _dashCooldownTimer <= 0) {
			_dashCooldownTimer = _dashCooldown;
			_rb.AddForce(_dashSpeed * _dashDirection, ForceMode2D.Impulse);
		}


	}

	public void Stun(float stunDuration)
	{
		_canMove = false;
		if (_currentStun != null)
			StopCoroutine(_currentStun);

		_currentStun = StartCoroutine(PlayerStun(stunDuration));

	}

	IEnumerator PlayerStun(float duration)
	{
		yield return new WaitForSeconds(duration);
		_canMove = true;
		_currentStun = null;

	}
}
