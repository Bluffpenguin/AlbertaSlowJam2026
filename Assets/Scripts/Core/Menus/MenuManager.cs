using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance { get; private set; }
	public string currentScene;
	private bool _paused;
	public bool inGame;

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
		//Debug.Log($"Current scene is: {currentScene}");
	}

	public void Pause_and_Unpause()
	{
		if (_paused && inGame)
		{
			SceneManager.UnloadSceneAsync("PauseMenu");
			Time.timeScale = 1;
		}
		else
		{
			Time.timeScale = 0;
			SceneManager.LoadScene("PauseMenu",LoadSceneMode.Additive);
		}
	}
}
