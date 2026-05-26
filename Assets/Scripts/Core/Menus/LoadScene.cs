using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // NOTE: This is applied to menu buttons that send you to different scenes.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void LoadSelectedScene(string wantedSceneName)
	{
		SceneManager.LoadScene(wantedSceneName, LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync(MenuManager.Instance.currentScene);
        MenuManager.Instance.currentScene = SceneManager.GetActiveScene().name;
		Debug.Log($"Loaded: {MenuManager.Instance.currentScene}");
	}
}
