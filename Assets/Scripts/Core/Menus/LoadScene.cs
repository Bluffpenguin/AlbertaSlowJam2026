using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // NOTE: This is applied to menu buttons that send you to different scenes.
    [SerializeField] private string wantedSceneName;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void LoadSelectedScene()
	{
		SceneManager.LoadScene(wantedSceneName);
		SceneManager.UnloadSceneAsync(MenuManager.Instance.currentScene);
		MenuManager.Instance.currentScene = wantedSceneName;
		//Debug.Log($"Loaded: {MenuManager.Instance.currentScene}");
	}

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Fuck this shit, I'm out");
    }

    /// <summary>
    /// Subject to change (will / can remove or alter for better pausing)
    /// </summary>
    public void CloseCurrentScene()
    {
        SceneManager.UnloadSceneAsync("PauseMenu");
    }
}
