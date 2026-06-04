using System.Linq;
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

    [SerializeField] private Image fadeToBlack;

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
	}
	void Start()
    {
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
}
