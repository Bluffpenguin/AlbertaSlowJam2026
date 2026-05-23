using UnityEditor;
using UnityEngine;

public class WaypointGenerator : MonoBehaviour
{
	[Header("Grid Size")]
	[SerializeField] float length = 10;
	[SerializeField] float width = 10;

	[Header("Waypoints")]
	[SerializeField] float separationDistance = 0.5f;
	[SerializeField] float agentSize = 1;
	[SerializeField] GameObject wpObject;
	[SerializeField] bool diagonalLinks = true;
	[SerializeField] LayerMask groundMask;
	[SerializeField] LayerMask obstacleMask;
	
	List<GameObject> wps = new List<GameObject>();
	Node[,] nodes;
	int numOfXPos = 0;
	int numOfYPos = 0;
	[SerializeField] bool generated = false;


	public void Generate()
    {
		
		if (generated)
		{
			DeleteWaypoints();
		}
		
		float leftBound = -(length / 2);
		float upperBound = (width / 2);

		numOfXPos = Mathf.FloorToInt(length / separationDistance) + 1;
		numOfYPos = Mathf.FloorToInt(width / separationDistance) + 1;

		nodes = new Node[numOfXPos, numOfYPos];

		Debug.Log("Starting Generation");
		for (int i = 0; i < numOfXPos; i++)
		{
			float xPos = leftBound + (i * separationDistance);

			for (int j = 0; j < numOfYPos; j++)
			{
				float yPos = upperBound - (j * separationDistance);

				// Check for ground and obstacles
				if (IsPosValid(new Vector2(xPos, yPos)))
				{
					Debug.Log("Placing Waypoint");
					GameObject wpObj = Instantiate(wpObject, new Vector3(xPos, yPos), Quaternion.identity, this.transform);
					nodes[i, j] = new Node(wpObj);
				}
				else
					nodes[i, j] = null;
			}
		}

		GenerateLinks();
		generated = true;
	}

	public void GenerateLinks()
	{
		if (numOfXPos == 0 || numOfYPos == 0) return;


		for (int i = 0; i < numOfXPos; i++)
		{
			for (int j = 0; j < numOfYPos; j++)
			{
				Node node = nodes[i, j];

				// Create edges
				if (i - 1 >= 0 && nodes[i - 1, j] != null)
					node.edgeList.Add(new Edge(node, nodes[i - 1, j]));

				if (i + 1 < numOfXPos && nodes[i + 1, j] != null)
					node.edgeList.Add(new Edge(node, nodes[i + 1, j]));

				if (j - 1 >= 0 && nodes[i, j - 1] != null)
					node.edgeList.Add(new Edge(node, nodes[i, j - 1]));

				if (j + 1 < numOfYPos && nodes[i, j + 1] != null)
					node.edgeList.Add(new Edge(node, nodes[i, j + 1]));

				if (diagonalLinks)
				{
					// Implement diagonals
				}
			}
		}
	}

	

	bool IsPosValid(Vector2 position)
	{
		if (Physics2D.OverlapPoint(position, groundMask) &&
			!Physics2D.OverlapBox(position, new Vector2(agentSize, agentSize), 0, obstacleMask))
		{
			Debug.Log("Valid Position");
			return true;
		}
		return false;
	}

	public void DeleteWaypoints()
	{
		if (numOfXPos == 0 || numOfYPos == 0) return;
		Debug.Log("Deleting Previous Waypoints");
		foreach (Node node in nodes)
		{
			GameObject obj = node.getId();
			DestroyImmediate(obj);
		}
		nodes = null;
		numOfXPos = 0;
		numOfYPos = 0;
		generated = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
		Gizmos.DrawCube(this.transform.position, new Vector3(length, width, 1));
		Gizmos.color = Color.white;

	}

	private void OnDrawGizmosSelected()
	{
		if (generated)
		{
			Gizmos.color = Color.yellow;
			foreach (Node node in nodes)
			{
				foreach (Edge edge in node.edgeList)
				{
					Vector3 start = edge.startNode.getId().transform.position;
					Vector3 end = edge.endNode.getId().transform.position;
					Gizmos.DrawLine(start, end);
				}
			}
			Gizmos.color = Color.white;
		}
		
	}
}
