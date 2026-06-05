using UnityEngine;

public class UI_Stun : MonoBehaviour
{
    [SerializeField] private Transform fill;
    [SerializeField] private Transform iconObj;
    [SerializeField] private Vector2 offset;
    float _stunProgess = 0, _stunDuration = 0;
    bool _isStunned = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		iconObj.gameObject.SetActive(false);
		fill.localScale = new Vector3(fill.localScale.x, 0, 1);
	}

	private void FixedUpdate()
	{
		if (!_isStunned) return;

		_stunProgess -= Time.fixedDeltaTime;

		float amountFilled = _stunProgess / _stunDuration;
		amountFilled = Mathf.Clamp(amountFilled, 0f, 1f);
		fill.localScale = new Vector3(fill.localScale.x, amountFilled, 1);

		if (amountFilled <= .01f)
		{
			_isStunned = false;
			iconObj.gameObject.SetActive(false);
		}
	}

	public void OnStun(float duration, Vector3 pos)
	{
		
		_stunProgess = duration;
		_stunDuration = duration;
		_isStunned = true;


		fill.localScale = new Vector3(fill.localScale.x, 1, 1);
		iconObj.position = pos + (Vector3)offset;
		iconObj.gameObject.SetActive(true);
	}
}
