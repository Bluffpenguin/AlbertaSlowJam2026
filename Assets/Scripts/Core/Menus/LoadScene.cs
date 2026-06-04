using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	// NOTE: This is applied to menu buttons that send you to different scenes.
	[SerializeField] private string wantedSceneName;

	public void LoadSelectedScene()
	{
		if (wantedSceneName.Contains("WorkShop"))
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayGameButtonWhistle, this.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonHiss, this.transform.position);
		}

			SceneManager.LoadScene(wantedSceneName);

		MenuManager.Instance.currentScene = wantedSceneName;

		if (MenuManager.Instance.currentScene.Contains("WorkShop"))
		{
			MenuManager.Instance.inGame = true;
		}
		//Debug.Log($"Loaded: {MenuManager.Instance.currentScene}");
	}

	public void LoadSettings()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonHiss, this.transform.position);
		SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Additive);
		MenuManager.Instance.currentScene = "SettingsMenu";
	}

	public void CloseScene()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonHiss, this.transform.position);
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
