using UnityEngine.Events;

public class InvokeOnDayStart : MonoBehaviour
{
	[SerializeField, Min(0)] private int _dayIndex = 0;
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
		if (dayIndex == _dayIndex)
			_onDayStart.Invoke();
	}
}
