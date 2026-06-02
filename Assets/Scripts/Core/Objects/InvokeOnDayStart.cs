using UnityEngine.Events;
using UnityEngine.Serialization;

public class InvokeOnDayStart : MonoBehaviour
{
	[FormerlySerializedAs("_dayIndex")]
	[SerializeField, Min(0)] private int _dayIndex = 0;
	[SerializeField, Min(1)] private int _length = 1;
	[SerializeField] private UnityEvent _onDayStart = new();

	public event UnityAction OnDayStart {
		add => _onDayStart.AddListener(value);
		remove => _onDayStart.RemoveListener(value);
	}

	protected virtual void OnEnable()
	{
		GameManager.Instance.AddListener(this);
	}

	protected virtual void OnDisable()
	{
		GameManager.Instance.RemoveListener(this);
	}

	public virtual void StartDay(int dayIndex)
	{
		var dayRange = new RangeInt(_dayIndex, _length);
		if (dayRange.Contains(dayIndex))
			_onDayStart.Invoke();
	}
}

public static class RangeHelpers
{
	public static bool Contains(this RangeInt range, int value)
	{
		return range.start <= value && value < range.end;
	}
}
