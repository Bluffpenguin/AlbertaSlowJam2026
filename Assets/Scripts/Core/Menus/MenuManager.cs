using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance { get; private set; }
	public string currentScene;

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
}
