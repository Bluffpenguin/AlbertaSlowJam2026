using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class LoadScene : MonoBehaviour
{
	[SerializeField] private GameMusic gameMusic;
	// NOTE: This is applied to menu buttons that send you to different scenes.
	[SerializeField] private string wantedSceneName;

	public void PlayGame()
	{
		AudioManager.Instance.SetGameMusic(gameMusic);
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayGameButtonWhistle, this.transform.position);
		MenuManager.Instance.inGame = true;
		SceneManager.LoadScene("WorkShop");
	}

	public void ToMainMenu()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonHiss, this.transform.position);
		MenuManager.Instance.inGame = false;
		AudioManager.Instance.SetGameMusic(gameMusic);
		SceneManager.LoadScene("MainMenu");
	}

	public void LoadSelectedScene()
	{
		if (SceneManager.GetSceneByName(wantedSceneName).isLoaded) return;

		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonHiss, this.transform.position);

		
		SceneManager.LoadScene(wantedSceneName);

		MenuManager.Instance.currentScene = wantedSceneName;

		if (MenuManager.Instance.currentScene.Contains("WorkShop"))
		{
			
		}
		//Debug.Log($"Loaded: {MenuManager.Instance.currentScene}");
	}

	public void LoadSettings()
	{
		if (SceneManager.GetSceneByName("SettingsMenu").isLoaded) return;

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
