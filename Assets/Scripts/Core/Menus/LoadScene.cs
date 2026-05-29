using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // NOTE: This is applied to menu buttons that send you to different scenes.
    [SerializeField] private string wantedSceneName;

	public void LoadSelectedScene()
	{
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

	public void Unpause()
	{
		MenuManager.Instance.Pause_and_Unpause();
	}

    public void QuitGame()
    {
        Application.Quit();
        //Debug.Log("Fuck this shit, I'm out");
    }
}
