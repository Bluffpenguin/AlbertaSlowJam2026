using TMPro;

public class HUD : MonoBehaviour
{
	public static HUD Instance { get; private set; }

	[SerializeField] private TextMeshProUGUI _playerMoneyLabel;
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

		_playerMoneyLabel.text = $"${GameManager.Instance.MoneyMadeToday}/{GameManager.Instance.TodaysQuota}";
		_currentDayLabel.text = ((DayOfWeek)GameManager.Instance.DayIndex).ToString();

		_clock.SetHours(GameManager.Instance.WakeUpHour, GameManager.Instance.EndOfDayHour);
		_clock.SetTime(GameManager.Instance.CurrentHour, GameManager.Instance.CurrentMinute);
	}
}
