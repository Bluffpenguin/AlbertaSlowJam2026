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
    Vector3 lastDungeonPos;
    bool activeDungeon = false;
    Camera sceneCamera;
    Vector3 cameraLocalPos;

    GameObject dungeonGenerator;
    CorridorFirstGenerator corridorFirstGenerator;

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
        shipPlayer = Player.Instance.transform;

        sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        sceneCamera.transform.SetParent(shipPlayer);
        cameraLocalPos = sceneCamera.transform.localPosition;
        Scene dungeonScene = SceneManager.GetSceneByName("TransitionTest"); //TEMP
        dungeonGenerator = dungeonScene.GetRootGameObjects().Where(obj => obj.name == "Corridor First").First();
        corridorFirstGenerator = dungeonGenerator.GetComponent<CorridorFirstGenerator>();
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
        shipPlayer.gameObject.SetActive(false);
        if (activeDungeon)
        {
			StartCoroutine(TransportToDungeon());
		}
        else
        {
            
            StartCoroutine(TransportToNewDungeon());
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
        shipPlayer.gameObject.SetActive(true);
        MoveCamera(shipPlayer);
		EndTransition.Invoke();
	}

	IEnumerator TransportToDungeon()
	{
		BeginTransition.Invoke();
		yield return new WaitForSeconds(1);
		EndTransition.Invoke();
	}

	IEnumerator TransportToNewDungeon()
	{
		BeginTransition.Invoke();
		yield return new WaitForSeconds(1);
		corridorFirstGenerator.Clear();
		corridorFirstGenerator.Generate();
        shipPlayer.gameObject.SetActive(false);
		dungeonPlayer = Player.Instance.transform;
        MoveCamera(dungeonPlayer);
		EndTransition.Invoke();
	}
}
