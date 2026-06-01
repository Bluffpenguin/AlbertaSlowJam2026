using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	public static HUD Instance { get; private set; }

	[SerializeField] private TextMeshProUGUI _currentDayLabel;
	[SerializeField] private Clock _clock;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (GameManager.Instance == null)
			return;

		_currentDayLabel.text = ((DayOfWeek)GameManager.Instance.DayIndex).ToString();

		_clock.SetHours(GameManager.Instance.WakeUpHour, GameManager.Instance.EndOfDayHour);
		_clock.SetTime(GameManager.Instance.CurrentHour, GameManager.Instance.CurrentMinute);
	}
}
