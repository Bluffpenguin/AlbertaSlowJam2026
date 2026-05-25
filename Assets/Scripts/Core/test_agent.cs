using UnityEngine;
using UnityEngine.InputSystem;

public class test_agent : MonoBehaviour
{
    [SerializeField] PathfindingManager manager;
    [SerializeField] float speed = 2.0f;
    [SerializeField] GameObject startObj;
    [SerializeField] GameObject endObj;
    [SerializeField] List<Node> path = new List<Node>();
    [SerializeField] float accuracy = 0.25f;
    int currentWP = 0;
    GameObject currentNode = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

	private void Update()
	{
		if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            if (startObj != null && endObj != null)
            {
                move(startObj, endObj);
            }
        }
	}
	// Update is called once per frame
	void LateUpdate()
    {
        if (path.Count == 0 || currentWP == path.Count)
            return;

		if (Vector2.Distance(path[currentWP].getId().transform.position, this.transform.position) < accuracy)
		{
			currentNode = path[currentWP].getId();
			currentWP++;
		}

		if (currentWP < path.Count)
		{
			Transform goal = path[currentWP].getId().transform;
			Vector2 lookAtGoal = new Vector2(goal.position.x, goal.position.y);

			Vector2 direction = lookAtGoal - (Vector2)this.transform.position;
			

			this.transform.Translate(direction * speed * Time.deltaTime);
		}
	}

    public void move(GameObject start, GameObject goal)
    {
        currentWP = 0;
		path.Clear();

        Node startNode = manager.GetNode(start);
        if (startNode == null) return;

        Node endNode = manager.GetNode(goal);
        if (endNode == null) return;

		if (manager.pf.AStar(startNode, endNode))
        {
            path = manager.pf.pathList;
            transform.position = start.transform.position;
        }

	}
}
