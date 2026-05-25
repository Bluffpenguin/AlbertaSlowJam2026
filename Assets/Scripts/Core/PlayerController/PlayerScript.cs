using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }
    public InputSystem_Actions playerInput;
    [SerializeField] private float _moveSpeed = 10;
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
		playerInput.Enable();
	}
	private void OnDisable()
	{
		playerInput.Disable();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        _moveDir = playerInput.Player.Move.ReadValue<Vector2>();
		Vector2 displacement = _moveSpeed * Time.fixedDeltaTime * _moveDir;

		_rb.AddForce(displacement,ForceMode2D.Impulse);
	}
}
