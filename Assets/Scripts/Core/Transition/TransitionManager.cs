using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    internal UnityEvent TransitionToShip = new UnityEvent();
	internal UnityEvent TransitionToDungeon = new UnityEvent();
	internal UnityEvent BeginTransition = new UnityEvent();
    internal UnityEvent MidShipTransition = new UnityEvent();
	internal UnityEvent MidDungeonTransition = new UnityEvent();
	internal UnityEvent EndTransition = new UnityEvent();
    internal UnityEvent<string> GameOver = new UnityEvent<string>();
	internal UnityEvent SleepToNextDay = new UnityEvent();

    [SerializeField] private Image fadeToBlack;

    [Header("Game Over Objs")]
    [SerializeField] private Image gameOverScreen;
    [SerializeField] private GameObject gameOverButtons;
	[SerializeField] private TextMeshProUGUI gameOverText;

    Transform shipPlayer;
    Transform dungeonPlayer;
    Camera sceneCamera;
    Vector3 cameraLocalPos;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        TransitionToShip.AddListener(ToShip);
        TransitionToDungeon.AddListener(ToDungeon);
		GameOver.AddListener(OnGameOver);
		SleepToNextDay.AddListener(OnSleep);
	}
	void Start()
    {
        gameOverScreen.gameObject.SetActive(false);
        gameOverButtons.SetActive(false);
        dungeonPlayer = shipPlayer = Player.Instance.transform;

        sceneCamera = Camera.main;
        sceneCamera.transform.SetParent(shipPlayer);
        cameraLocalPos = sceneCamera.transform.localPosition;
	}

    void ToShip()
    {
		StartCoroutine(TransportToShip(3));
	}

    void ToDungeon()
    {       
			StartCoroutine(TransportToDungeon(3));
    }

	void OnSleep() { StartCoroutine(Sleep()); }
	void OnGameOver(string message) { StartCoroutine(GameOverTransition(message)); }

	public void OnGameOverRestartClicked() { StartCoroutine(GameOverRestart()); }
	public void OnGameOverExitClicked() { return; }

	void MoveCamera(Transform player)
    {
        sceneCamera.transform.SetParent(player);
        sceneCamera.transform.localPosition = cameraLocalPos;
    }

    IEnumerator TransportToShip(float time)
    {
		Player.Controller.PlayerInput.Disable();
		fadeToBlack.enabled = true;
        BeginTransition.Invoke();
		while (fadeToBlack.color.a < 1)
		{
			Color color = fadeToBlack.color;
			color.a += (time / 3) * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

        MidShipTransition.Invoke();
		dungeonPlayer.gameObject.SetActive(false);
        Player.Controller.transform.position = Vector3.zero; // Hacky
        

        shipPlayer.gameObject.SetActive(true);
        MoveCamera(shipPlayer);
        yield return new WaitForSeconds(time / 3);
		Player.Controller.PlayerInput.Enable();
		while (fadeToBlack.color.a > 0)
		{
			Color color = fadeToBlack.color;
			color.a -= (time / 3) * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

		EndTransition.Invoke();
        fadeToBlack.enabled = false;
	}

	IEnumerator TransportToDungeon(float time)
	{
		
		Player.Controller.PlayerInput.Disable();
		fadeToBlack.enabled = true;
        BeginTransition.Invoke();
        while (fadeToBlack.color.a < 1)
        {
            Color color = fadeToBlack.color;
            color.a += (time / 3) * Time.deltaTime;
            fadeToBlack.color = color;
            yield return null;
        }
		
        MidDungeonTransition.Invoke();
		shipPlayer.gameObject.SetActive(false);
        Player.Controller.transform.position = 10 * Vector3.forward;

		if (dungeonPlayer == null)
        {
            dungeonPlayer = Player.Instance.transform;
        }
        dungeonPlayer.gameObject.SetActive(true);
        MoveCamera(dungeonPlayer);
        yield return new WaitForSeconds(time/3);
		Player.Controller.PlayerInput.Enable();
		while (fadeToBlack.color.a > 0)
		{
			Color color = fadeToBlack.color;
			color.a -= (time / 3) * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

		EndTransition.Invoke();
		
		fadeToBlack.enabled = false;
	}

    IEnumerator GameOverTransition(string gameOverMessage)
    {

		// Ensure the screen and text is transparent
		Color transparent = gameOverScreen.color;
        transparent.a = 0;
        gameOverScreen.color = transparent;
		transparent = gameOverText.color;
		transparent.a = 0;
		gameOverText.color = transparent;

		Player.Controller.PlayerInput.Disable();
		gameOverScreen.gameObject.SetActive(true);
		gameOverText.text = gameOverMessage;
		gameOverButtons.SetActive(false);
		while (gameOverScreen.color.a < 1 || gameOverText.color.a < 1)
		{
			Color screenColor = gameOverScreen.color;
			screenColor.a += 1 * Time.unscaledDeltaTime;
			gameOverScreen.color = screenColor;

			Color textColor = gameOverText.color;
			textColor.a += 1 * Time.unscaledDeltaTime;
			gameOverText.color = textColor;
			yield return null;
		}

        gameOverButtons.SetActive(true);
	}

    IEnumerator GameOverRestart()
    {
		Color whole = fadeToBlack.color;
		whole.a = 1;
		fadeToBlack.color = whole;

		fadeToBlack.enabled = true;
		gameOverButtons.SetActive(false);
		gameOverScreen.gameObject.SetActive(false);

		MidShipTransition.Invoke();
		dungeonPlayer.gameObject.SetActive(false);
		Player.Controller.transform.position = Vector3.zero; // Hacky
		GameManager.Instance.ResetGame();
		GameManager.Instance.MoveToNextDay();

		shipPlayer.gameObject.SetActive(true);
		MoveCamera(shipPlayer);
	
		Player.Controller.PlayerInput.Enable();
		Time.timeScale = 1;
		while (fadeToBlack.color.a > 0)
		{
			Color color = fadeToBlack.color;
			color.a -= (1) * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

		EndTransition.Invoke();
		fadeToBlack.enabled = false;
	}

	IEnumerator Sleep()
	{
		Player.Controller.PlayerInput.Disable();
		fadeToBlack.enabled = true;
		BeginTransition.Invoke();
		while (fadeToBlack.color.a < 1)
		{
			Color color = fadeToBlack.color;
			color.a += 0.5f * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

		GameManager.Instance.MoveToNextDay();
		Player.Controller.PlayerInput.Enable();
		while (fadeToBlack.color.a > 0)
		{
			Color color = fadeToBlack.color;
			color.a -= 0.5f * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

		EndTransition.Invoke();

		fadeToBlack.enabled = false;
	}
}
