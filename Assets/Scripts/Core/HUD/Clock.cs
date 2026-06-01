using UnityEngine.UI;

public class Clock : MonoBehaviour
{
	[SerializeField]
	[Range(0, 1)] private float _progress;
	[Space]
	[SerializeField]
	[Range(0, 1)] private float _maskProgress;
	[SerializeField]
	[Range(0, 1)] private float _sellHoursProgress;
	[Space]
	[SerializeField] private Image _hourHand;
	[SerializeField] private Image _minuteHand;
	[SerializeField] private Image _mask, _sellHoursSegment, _afterHoursSegment;

	private const float HOURS_PER_DAY = 24f, MINUTES_PER_HOUR = 60f,
		MINUTES_PER_DAY = HOURS_PER_DAY * MINUTES_PER_HOUR;

	public void SetTime(float hour, float minute)
	{
		_progress = (hour / HOURS_PER_DAY) + (minute / MINUTES_PER_DAY);
	}

	public void SetHours(int wakeUp, int closingTime)
	{
		_maskProgress = wakeUp / HOURS_PER_DAY;
		_sellHoursProgress = (closingTime - wakeUp) / HOURS_PER_DAY;
	}

	public void Update()
	{
		var hour = _progress;
		_hourHand.transform.eulerAngles = new Vector3(0, 0, -360f * hour);
		var minute = _progress * MINUTES_PER_DAY / MINUTES_PER_HOUR;
		_minuteHand.transform.eulerAngles = new Vector3(0, 0, -180f * minute);

		if (_mask != null) {
			_mask.fillAmount = 1 - _maskProgress;
		}
		if (_sellHoursSegment != null) {
			_sellHoursSegment.fillAmount = _maskProgress + _sellHoursProgress;
		}
	}

	public void OnValidate()
	{
		Update();
		
	}
}
