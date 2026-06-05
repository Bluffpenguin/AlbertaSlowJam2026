using UnityEngine.SceneManagement;
using FMOD.Studio;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private List<InvokeOnDayStart> _listeners = new();
	[SerializeField] private string _dungeonScene = "Dungeon";
	[SerializeField] private string _hudScene = "HUD";

	[Header("Game Settings")]
	[SerializeField] public bool AdvanceClockInShip = true;
	[Range(1, 12)] public int WakeUpHour = 6;
	[Range(13, 24)] public int EndOfDayHour = 18;
	[Tooltip("The ratio between realtime seconds to in-game minutes.")]
	public float SecondsPerMinute = 1;
	[SerializeField] private int[] _quotas = new int[1] { 40 };

	public enum GameState
	{
		InDungeon,
		InShip,
		EndScreen
	}
	public GameState gameState = GameState.InShip;
	bool dayStarted = false;

	[Header("Dynamic Data")]
	public int DayIndex = -1;
	public float TimeElapsed;
	public float CurrentHour, CurrentMinute;
	public bool AdvanceClock = false;
	public int PlayerMoney;
	public int PlayerMoneyOverall = 0;
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
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		Instance = this;
		this.transform.SetParent(null);
		DontDestroyOnLoad(this.gameObject);

		TransitionManager.Instance.TransitionToShip.AddListener(SwitchToShip);
		TransitionManager.Instance.TransitionToDungeon.AddListener(SwitchToDungeon);
	}

	private void Start()
	{
		// Testing
		StartGame();
	}

	public void Update()
	{
		if (!AdvanceClock || !dayStarted)
		{
			return;
		}

		if (!AdvanceClockInShip && gameState == GameState.InShip) return;

		TimeElapsed += Time.deltaTime;
		var minutesPassed = TimeElapsed / SecondsPerMinute;
		CurrentHour = WakeUpHour + (minutesPassed / 60);
		CurrentMinute = minutesPassed % 60;

		if (CurrentHour >= EndOfDayHour)
		{
			AdvanceClock = false;
			EndDay();
		}
	}

	public async void StartGame()
	{
		await SceneManager.LoadSceneAsync(_hudScene, LoadSceneMode.Additive);
		await SceneManager.LoadSceneAsync(_dungeonScene, LoadSceneMode.Additive);
		ResetGame();
		await Awaitable.MainThreadAsync();
		MoveToNextDay();
	}

	public void ResetGame()
	{
		InventoryViewManager.Instance.ClearAllInventories();
		Player.Inventory.Clear();
		PlayerMoneyOverall = 0;
		DayIndex = -1;
		TimeElapsed = 0;
		gameState = GameState.InShip;
	}

	public void EndDay()
	{
		PlayerMoneyOverall += PlayerMoney;
		if (PlayerMoney >= _quotas[DayIndex] && gameState == GameState.InShip)
		{
			// Passed, proceed to next day
			if (DayIndex + 1 >= DaysToWin)
			{
				TransitionManager.Instance.GameWin.Invoke("You win");
			}
			else
				TransitionManager.Instance.SleepToNextDay.Invoke();
		}
		else
		{
			// Failed quota
			EndGame();
		}
	}

	public void MoveToNextDay()
	{
		Debug.Log("Moving to next day");
		AdvanceClock = false;
		DayIndex++;
		TimeElapsed = 0;
		PlayerMoney = 0;
		dayStarted = false;

		TodaysQuota = _quotas[DayIndex];

		CurrentHour = WakeUpHour;
		CurrentMinute = 0;

		Player.Inventory.Clear();

		foreach (var listener in _listeners)
		{
			listener.StartDay(DayIndex);
		}

		AdvanceClock = true;
	}

	void SwitchToDungeon()
	{
		gameState = GameState.InDungeon;
		AudioManager.Instance.SetGameMusic(GameMusic.Scavenge);
		dayStarted = true;
	}
	void SwitchToShip() 
	{
		gameState = GameState.InShip;
		AudioManager.Instance.SetGameMusic(GameMusic.Ship);
	}

	public void EndGame()
	{
		Time.timeScale = 0;
		


		if (gameState == GameState.InDungeon)
		{
			TransitionManager.Instance.GameOver.Invoke("Abandoned");
		}
		else { TransitionManager.Instance.GameOver.Invoke("Unemployed"); }

		gameState = GameState.EndScreen;
		Debug.Log("Game has ended");
	}

}
