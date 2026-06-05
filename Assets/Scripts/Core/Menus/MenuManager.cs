using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance { get; private set; }
	public string currentScene;
	[SerializeField] private bool _paused;
	public bool Paused => _paused;
	public bool inGame;
	[SerializeField] private GameMusic gameMusic;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(Instance);
		}
		currentScene = SceneManager.GetActiveScene().name;
		inGame = false;
		//Debug.Log($"Current scene is: {currentScene}");
	}

	private void Start()
	{
		AudioManager.Instance.InitializeMusic(FMODEvents.Instance.GameMusic);
	}

	private void Update()
	{
		if (Keyboard.current.eKey.wasPressedThisFrame && InventoryViewManager.Instance != null)
		{
			if (inGame == false)
			{
				InventoryViewManager.Instance.CloseAllOpenViews();
			}
		}
	}

	public void Pause_and_Unpause()
	{
		if (!inGame)
			return;

		if (Paused)
		{
			SceneManager.UnloadSceneAsync("PauseMenu");
			Time.timeScale = 1;
			_paused = false;
		}
		else
		{
			Time.timeScale = 0;
			SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
			_paused = true;
		}
	}
}
