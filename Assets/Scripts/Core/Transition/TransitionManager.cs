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
    internal UnityEvent EndTransition = new UnityEvent();

    [SerializeField] private Image fadeToBlack;

    Transform shipPlayer;
    Transform dungeonPlayer;
    bool activeDungeon = false;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToShip()
    {
		StartCoroutine(TransportToShip());
	}

    void ToDungeon()
    {
        //activeDungeon = true; //TEMP 
        
			StartCoroutine(TransportToDungeon());
        if (activeDungeon)
        {
		}
        
    }

    void MoveCamera(Transform player)
    {
        sceneCamera.transform.SetParent(player);
        sceneCamera.transform.localPosition = cameraLocalPos;
    }

    IEnumerator TransportToShip()
    {
        BeginTransition.Invoke();
        yield return new WaitForSeconds(1);
        dungeonPlayer.gameObject.SetActive(false);
        Player.Controller.transform.position = Vector3.zero; // Hacky
        activeDungeon = false;
        shipPlayer.gameObject.SetActive(true);
        MoveCamera(shipPlayer);
		EndTransition.Invoke();
	}

	IEnumerator TransportToDungeon()
	{
		BeginTransition.Invoke();
		yield return new WaitForSeconds(1);
		shipPlayer.gameObject.SetActive(false);
        Player.Controller.transform.position = 10 * Vector3.forward;
        activeDungeon = true;
		if (dungeonPlayer == null)
        {
            dungeonPlayer = Player.Instance.transform;
        }
        dungeonPlayer.gameObject.SetActive(true);
        MoveCamera(dungeonPlayer);
		EndTransition.Invoke();
	}
}
