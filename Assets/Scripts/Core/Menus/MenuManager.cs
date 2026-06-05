using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance { get; private set; }
	public string currentScene;
	[SerializeField] private bool _paused;
	public bool Paused => _paused;
	public bool inGame;
	private EventInstance _menuMusic;

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
		_menuMusic = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.MenuMusic);
	}

	private void FixedUpdate()
	{
		UpdateMusic();
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

	private void UpdateMusic()
	{
		if (!inGame)
		{
			_menuMusic.getPlaybackState(out PLAYBACK_STATE playbackState);

			if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
			{
				_menuMusic.start();
			}
		}
		else
		{
			_menuMusic.stop(STOP_MODE.ALLOWFADEOUT);

		}
	}
}
