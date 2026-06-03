using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private List<InvokeOnDayStart> _listeners = new();
	[SerializeField] private string _dungeonScene = "Dungeon";
	[SerializeField] private string _hudScene = "HUD";

	[Header("Game Settings")]
	[Range(1, 12)] public int WakeUpHour = 6;
	[Range(13, 24)] public int EndOfDayHour = 18;
	[Tooltip("The ratio between realtime seconds to in-game minutes.")]
	public float SecondsPerMinute = 1;
	[SerializeField] private int[] _quotas = new int[1] { 40 };

	[Header("Dynamic Data")]
	public int DayIndex = -1;
	public float TimeElapsed;
	public float CurrentHour, CurrentMinute;
	public bool AdvanceClock = false;
	public int PlayerMoney;
	public int TodaysQuota;

	public int DaysToWin => _quotas.Length;

	public void AddListener(InvokeOnDayStart listener)
	{
		if (!_listeners.Contains(listener))
			_listeners.Add(listener);
	}

	public void RemoveListener(InvokeOnDayStart listener)
	{
		_listeners.Remove(listener);
	}

	private void Awake()
	{
		if (Instance != null && Instance != this) {
			Destroy(this.gameObject);
			return;
		}

		Instance = this;
		this.transform.SetParent(null);
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		// Testing
		StartGame();
	}

	public void Update()
	{
		if (!AdvanceClock) {
			return;
		}

		TimeElapsed += Time.deltaTime;
		var minutesPassed = TimeElapsed / SecondsPerMinute;
		CurrentHour = WakeUpHour + (minutesPassed / 60);
		CurrentMinute = minutesPassed % 60;
	}

	public async void StartGame()
	{
		await SceneManager.LoadSceneAsync(_hudScene, LoadSceneMode.Additive);
		await SceneManager.LoadSceneAsync(_dungeonScene, LoadSceneMode.Additive);
		ResetGame();
		await Awaitable.MainThreadAsync();
		MoveToNextDay();
		AudioManager.Instance.InitializeMusic(FMODEvents.Instance.ShopMusic);
	}

	public void ResetGame()
	{
		DayIndex = -1;
		TimeElapsed = 0;
	}

	public void MoveToNextDay()
	{
		Debug.Log("Moving to next day");
		AdvanceClock = false;
		DayIndex++;
		TimeElapsed = 0;
		PlayerMoney = 0;

		if (DayIndex >= DaysToWin) {
			EndGame();
			return;
		}

		TodaysQuota = _quotas[DayIndex];

		foreach (var listener in _listeners) {
			listener.StartDay(DayIndex);
		}

		AdvanceClock = true;
	}

	public void EndGame()
	{
		Debug.Log("Game has ended");
	}
}
