using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UI_Dash : MonoBehaviour
{
	[SerializeField] Color rechargingColor;
	[SerializeField] Color chargedColor;
	[SerializeField] Image fillObj;
	[SerializeField] Transform _fillBar;
	bool _onCooldown = false;
	float _cooldownMax = 0, _cooldownProgress = 0;

	private void Start()
	{
		fillObj.color = chargedColor;
		_onCooldown = false;
	}

	public void OnDash(float cooldownTime)
	{
		_cooldownProgress = 0;
		_cooldownMax = cooldownTime;
		_onCooldown = true;

		fillObj.color = rechargingColor;
		_fillBar.localScale = new Vector3(_fillBar.localScale.x, 0, 1);
	}

	private void FixedUpdate()
	{
		if (!_onCooldown) return;

		_cooldownProgress += Time.fixedDeltaTime;

		float amountFilled = _cooldownProgress / _cooldownMax;
		amountFilled = Mathf.Clamp(amountFilled, 0f, 1f);
		_fillBar.localScale = new Vector3(_fillBar.localScale.x, amountFilled, 1);

		if (amountFilled >= .99f)
		{
			_onCooldown = false;
			fillObj.color = chargedColor;
		}
	}
}
