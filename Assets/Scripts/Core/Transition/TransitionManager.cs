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
	internal UnityEvent<string> GameWin = new UnityEvent<string>();
	internal UnityEvent SleepToNextDay = new UnityEvent();

    [SerializeField] private Image fadeToBlack;

    [Header("Game End Screen Objs")]
    [SerializeField] private Image gameEndScreen;
    [SerializeField] private GameObject gameOverButtons;
	[SerializeField] private GameObject gameWinButtons;
	[SerializeField] private TextMeshProUGUI gameEndText;
	[Space]
	[SerializeField] private GameObject runInfo;
	[SerializeField] private TextMeshProUGUI daysSurvivedText;
	[SerializeField] private TextMeshProUGUI totalScoreText;

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
		GameWin.AddListener(OnGameWin);
		SleepToNextDay.AddListener(OnSleep);
	}
	void Start()
    {
        gameEndScreen.gameObject.SetActive(false);
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
	public void OnGameOverExitClicked()
	{
		StartCoroutine(GameOverToMainMenu());
	}

	void OnGameWin(string message) { StartCoroutine(GameWinTransition(message)); }

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
		Color transparent = gameEndScreen.color;
        transparent.a = 0;
        gameEndScreen.color = transparent;
		transparent = gameEndText.color;
		transparent.a = 0;
		gameEndText.color = transparent;

		Player.Controller.PlayerInput.Disable();
		gameEndScreen.gameObject.SetActive(true);
		gameEndText.text = gameOverMessage;
		gameOverButtons.SetActive(false);
		while (gameEndScreen.color.a < 1 || gameEndText.color.a < 1)
		{
			Color screenColor = gameEndScreen.color;
			screenColor.a += 1 * Time.unscaledDeltaTime;
			gameEndScreen.color = screenColor;

			Color textColor = gameEndText.color;
			textColor.a += 1 * Time.unscaledDeltaTime;
			gameEndText.color = textColor;
			yield return null;
		}
		
		if (gameOverMessage == "Abandoned")
		{
			daysSurvivedText.text = "You survived " + GameManager.Instance.DayIndex.ToString() + " days";
			totalScoreText.text = "and made $" + GameManager.Instance.PlayerMoneyOverall.ToString();
		}
		else
		{
			daysSurvivedText.text = "You had a job for " + GameManager.Instance.DayIndex.ToString() + " days";
			totalScoreText.text = "and made $" + GameManager.Instance.PlayerMoneyOverall.ToString();
		}
		runInfo.SetActive(true);
		gameOverButtons.SetActive(true);
	}

    IEnumerator GameOverRestart()
    {
		Color whole = fadeToBlack.color;
		whole.a = 1;
		fadeToBlack.color = whole;

		fadeToBlack.enabled = true;
		gameOverButtons.SetActive(false);
		gameWinButtons.SetActive(false);
		runInfo.SetActive(false);
		gameEndScreen.gameObject.SetActive(false);

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

	IEnumerator GameOverToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");

		Color whole = fadeToBlack.color;
		whole.a = 1;
		fadeToBlack.color = whole;

		fadeToBlack.enabled = true;
		gameOverButtons.SetActive(false);
		gameWinButtons.SetActive(false);
		runInfo.SetActive(false);
		gameEndScreen.gameObject.SetActive(false);
		GameManager.Instance.StopMusic();
		Destroy(GameManager.Instance.gameObject);

		Player.Controller.PlayerInput.Enable();
		Time.timeScale = 1;
		while (fadeToBlack.color.a > 0)
		{
			Color color = fadeToBlack.color;
			color.a -= (1) * Time.deltaTime;
			fadeToBlack.color = color;
			yield return null;
		}

		fadeToBlack.enabled = false;
		Destroy(this.gameObject);
	}

	IEnumerator GameWinTransition(string gameOverMessage)
	{

		// Ensure the screen and text is transparent
		Color transparent = gameEndScreen.color;
		transparent.a = 0;
		gameEndScreen.color = transparent;
		transparent = gameEndText.color;
		transparent.a = 0;
		gameEndText.color = transparent;

		Player.Controller.PlayerInput.Disable();
		gameEndScreen.gameObject.SetActive(true);
		gameEndText.text = gameOverMessage;
		gameWinButtons.SetActive(false);
		while (gameEndScreen.color.a < 1 || gameEndText.color.a < 1)
		{
			Color screenColor = gameEndScreen.color;
			screenColor.a += 1 * Time.unscaledDeltaTime;
			gameEndScreen.color = screenColor;

			Color textColor = gameEndText.color;
			textColor.a += 1 * Time.unscaledDeltaTime;
			gameEndText.color = textColor;
			yield return null;
		}

		daysSurvivedText.text = "You survived the entire week";
		totalScoreText.text = "and made $" + GameManager.Instance.PlayerMoneyOverall.ToString();
		runInfo.SetActive(true);

		gameWinButtons.SetActive(true);
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
