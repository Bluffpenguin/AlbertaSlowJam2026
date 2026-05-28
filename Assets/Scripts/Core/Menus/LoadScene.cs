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
        if (MenuManager.Instance.Paused)
        {
            UnpauseGame();
        }

		SceneManager.LoadScene(wantedSceneName);
		
		MenuManager.Instance.currentScene = wantedSceneName;

        if (MenuManager.Instance.currentScene.Contains("Game"))
        {
			MenuManager.Instance.inGame = true;
		}
		//Debug.Log($"Loaded: {MenuManager.Instance.currentScene}");
	}

    public void CloseScene()
    {
		SceneManager.UnloadSceneAsync(MenuManager.Instance.currentScene);
	}

    public void UnpauseGame()
    {
        MenuManager.Instance.Pause_and_Unpause();
    }

    public void QuitGame()
    {
        Application.Quit();
        //Debug.Log("Fuck this shit, I'm out");
    }
}
