using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private List<InvokeOnDayStart> _listeners = new();

	[Header("Game Settings")]
	public float DayLength = 600f;
	[Range(0, 24)] public int StartingHour = 6;
	[Min(1)] public int DaysToWin = 7;

	[Header("Dynamic Data")]
	public int DayIndex = -1;
	public float TimeElapsed;
	public bool AdvanceClock = false;

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

	public void Update()
	{
		if (!AdvanceClock) {
			return;
		}
		
		TimeElapsed += Time.deltaTime;
		if (TimeElapsed >= DayLength) {
			// TODO: temporary, remove later
			MoveToNextDay();
		}
	}

	public void ResetGame()
	{
		DayIndex = -1;
		TimeElapsed = 0;
	}

	public void MoveToNextDay()
	{
		DayIndex++;
		TimeElapsed = 0;

		if (DayIndex >= DaysToWin) {
			EndGame();
			return;
		}

		foreach (var listener in _listeners) {
			listener.StartDay(DayIndex);
		}
	}

	public void EndGame()
	{
		Debug.Log("Game has ended");
	}
}
