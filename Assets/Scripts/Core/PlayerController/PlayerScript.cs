using UnityEngine;

public class Player : MonoBehaviour
{
    private InputSystem_Actions _playerInput;
    [SerializeField] private float _moveSpeed = 10;
    private Vector2 _moveDir;
    private Rigidbody2D _rb;


	private void Awake()
	{
        _playerInput = new();
        _rb = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		_playerInput.Enable();
	}
	private void OnDisable()
	{
		_playerInput.Disable();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        _moveDir = _playerInput.Player.Move.ReadValue<Vector2>();
		Vector2 displacement = _moveSpeed * Time.fixedDeltaTime * _moveDir;

		_rb.AddForce(displacement,ForceMode2D.Impulse);
	}
}
