using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] Transform _tooltipTransform;
    [SerializeField] TextMeshProUGUI _tooltipTextObj;
	[SerializeField] TextMeshProUGUI _tooltipPromptObj;
    [SerializeField] string _loadedTooltip;

    public static UnityEvent<Vector3, string, float> CallTooltip = new UnityEvent<Vector3, string, float>();
    public static UnityEvent DismisTooltip = new UnityEvent();
	bool _isTooltipOpen = false;
	float _timeToCompleteCurrentAction;
	float _actionProgress;

	InputAction _playerInteract;

	[Header("UI Animation")]
	[SerializeField] bool _useAnim;
	[SerializeField] RectTransform _fillBar;
	[Space]
	[SerializeField] Transform _gear1;
	[SerializeField] int _rotationSpeed1 = 10;
	Quaternion _gear1StartRotation;
	[Space]
	[SerializeField] Transform _gear2;
	[SerializeField] int _rotationSpeed2 = 10;
	Quaternion _gear2StartRotation;
	[Space]
	[SerializeField] Transform _gear3;
	[SerializeField] int _rotationSpeed3 = 10;
	Quaternion _gear3StartRotation;
	bool isInteractPressed = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
		CallTooltip.AddListener(OpenTooltip);
		DismisTooltip.AddListener(CloseTooltip);
		_tooltipTransform.gameObject.SetActive(false);
	}

	private void Start()
	{
		_playerInteract = Player.Controller.PlayerInput.Player.Interact;
		_gear1StartRotation = _gear1.rotation;
		_gear2StartRotation = _gear2.rotation;
		_gear3StartRotation = _gear3.rotation;
	}
	private void Update()
	{
		if (!_isTooltipOpen) return;


		if (_playerInteract.WasPressedThisFrame())
		{
			isInteractPressed = true;
		}
		else if (_playerInteract.WasReleasedThisFrame())
		{
			isInteractPressed = false;
			_fillBar.localScale = new Vector3(0, _fillBar.localScale.y);
			_actionProgress = 0;
		}
	}

	private void FixedUpdate()
	{


		if (!_isTooltipOpen || !_useAnim) return;


		if (isInteractPressed)
		{
			_actionProgress += Time.fixedDeltaTime;

			_gear1.Rotate(new Vector3(0, 0, _rotationSpeed1 * Time.fixedDeltaTime));
			_gear2.Rotate(new Vector3(0, 0, _rotationSpeed2 * Time.fixedDeltaTime));
			_gear3.Rotate(new Vector3(0, 0, _rotationSpeed3 * Time.fixedDeltaTime));

			// Fill bar
			float amountFilled = _actionProgress / _timeToCompleteCurrentAction;
			amountFilled = Mathf.Clamp(amountFilled, 0f, 1f);
			_fillBar.localScale = new Vector3(amountFilled, _fillBar.localScale.y);

			if (amountFilled >= .99f)
			{
				isInteractPressed = false;
				_fillBar.localScale = new Vector3(0, _fillBar.localScale.y);
				CloseTooltip();
			}
		}
	}

	void OpenTooltip(Vector3 pos, string tip, float timeToComplete)
	{
		_loadedTooltip = tip;
		_tooltipTransform.position = pos;
		_tooltipPromptObj.text = "E";
		_tooltipTextObj.text = tip;
		_tooltipTransform.gameObject.SetActive(true);
		_timeToCompleteCurrentAction = timeToComplete;
		_isTooltipOpen = true;
	}

	void CloseTooltip()
	{
		_tooltipTransform.gameObject.SetActive(false);
		_loadedTooltip = null;
		_tooltipTextObj.text = "";
		_fillBar.localScale = new Vector3(0, _fillBar.localScale.y);
		_actionProgress = 0;
		_isTooltipOpen = false;
	}
}
