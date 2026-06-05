using FMOD.Studio;
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
	[SerializeField] private UI_Dash _dashUI;

	private Rigidbody2D _rb;
	private Animator _anim;
	private SpriteRenderer _spriteRenderer;
	private Vector3 _animDir;
	private Vector2 _moveDir;
	private Vector2 _dashDirection;
	private float _dashCooldownTimer;
	private EventInstance _playerFootsteps;

	private bool _canMove = true;
	Coroutine _currentStun = null;

	private void Awake()
	{
		_playerInput = new();
		_anim = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		_playerFootsteps = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootsteps);
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
		_playerInput.UI.Disable();
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
		_moveDir.y *= 0.5f;

		if (_moveDir.magnitude == 0) _rb.linearVelocity = Vector2.zero;

		Vector2 displacement = _moveSpeed * Time.fixedDeltaTime * _moveDir;
		_rb.AddForce(displacement, ForceMode2D.Impulse);
		if (_rb.linearVelocity != Vector2.zero) {
			_dashDirection = _rb.linearVelocity.normalized;
		}

		_dashCooldownTimer -= Time.fixedDeltaTime;
		if (_playerInput.Player.Dash.IsPressed() && _dashCooldownTimer <= 0) {
			_dashCooldownTimer = _dashCooldown;
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDash, this.transform.position);
			_rb.AddForce(_dashSpeed * _dashDirection, ForceMode2D.Impulse);
			_dashUI.OnDash(_dashCooldown);
		}

		UpdateSound(_moveDir);
		UpdateAnimation();
	}

	public void Stun(float stunDuration)
	{
		_canMove = false;
		if (_currentStun != null)
			StopCoroutine(_currentStun);

		_currentStun = StartCoroutine(PlayerStun(stunDuration));

	}

	void UpdateAnimation()
	{
		
		if (_rb.linearVelocity.magnitude > 0.05f)
		{
			// Player is moving
			_animDir = _rb.linearVelocity.normalized;
			_anim.SetBool("IsWalking", true);
			
			if (_animDir.x > 0 && _animDir.x > Mathf.Abs(_animDir.y))
			{
				// Face right
				_spriteRenderer.flipX = false;
			}
			else if (_animDir.x < 0 && Mathf.Abs(_animDir.x) > Mathf.Abs(_animDir.y))
			{
				// Face left
				_spriteRenderer.flipX = true;
			}
			else if (_animDir.y > 0)
			{
				// Face up

				//Placeholder
				if (_animDir.x > 0) { _spriteRenderer.flipX = false; }
				else _spriteRenderer.flipX = true;
			}
			else
			{
				// Face down

				//Placeholder
				if (_animDir.x > 0) { _spriteRenderer.flipX = false; }
				else _spriteRenderer.flipX = true;
			}
		}
		else
		{
			// Player is idle
			_anim.SetBool("IsWalking", false);

			if (_animDir.x > 0 && _animDir.x > Mathf.Abs(_animDir.y))
			{
				// Face right
				_spriteRenderer.flipX = false;
			}
			else if (_animDir.x < 0 && Mathf.Abs(_animDir.x) > Mathf.Abs(_animDir.y))
			{
				// Face left
				_spriteRenderer.flipX = true;
			}
			else if (_animDir.y > 0)
			{
				// Face up

				//Placeholder
				if (_animDir.x > 0) { _spriteRenderer.flipX = false; }
				else _spriteRenderer.flipX = true;
			}
			else
			{
				// Face down
				

				//Placeholder
				if (_animDir.x > 0) { _spriteRenderer.flipX = false; }
				else _spriteRenderer.flipX = true;
			}
		}
	}

	IEnumerator PlayerStun(float duration)
	{
		yield return new WaitForSeconds(duration);
		_canMove = true;
		_currentStun = null;

	}

	private void UpdateSound(Vector2 moveDir)
	{
		// start footsteps event if the player is moving
		if (moveDir != Vector2.zero && _canMove)
		{
			// get playback state
			PLAYBACK_STATE playbackState;
			_playerFootsteps.getPlaybackState(out playbackState);

			if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
			{
				_playerFootsteps.start();
			}
		}
		// otherwise stop the footsteps event
		else
		{
			_playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
		}
	}
}
