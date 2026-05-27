using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using JetBrains.Annotations;

[RequireComponent(typeof(Pathfinder))]
public class PathfindingManager : MonoBehaviour
{
	[Header("Waypoints")]
	[SerializeField] float agentSize = 1;
	[SerializeField] GameObject wpObject;
	[SerializeField] GameObject graphPrefab;
	[SerializeField] bool diagonalLinks = true;
	[SerializeField] LayerMask groundMask;
	[SerializeField] LayerMask obstacleMask;

	[Header("Testing")]
	[SerializeField] float test_length = 10;
	[SerializeField] float test_width = 10;
	[SerializeField] float test_seperationDist = 0.5f;
	[SerializeField] bool test_generated = false;

	List<Graph> graphs = new List<Graph>();

	Dictionary<Vector2Int, Node> nodeDictionary;
	Tilemap tileMap;
	bool links_generated = false;

	public Pathfinder pf;


	private void Start()
	{
		pf = GetComponent<Pathfinder>();
	}

	struct Graph
	{
		internal Node[,] nodes;
		internal float xDimension;
		internal float yDimension;
		internal GameObject obj;

		public Graph(Node[,] nodes, float xDimension, float yDimension, GameObject obj)
		{
			this.nodes = nodes;
			this.xDimension = xDimension;
			this.yDimension = yDimension;
			this.obj = obj;
		}
	}

	#region Graph Generation

	public int GenerateGrid(Vector2 position, float xDimension, float yDimension, float nodeSeperation)
	{

		GameObject graphObj = Instantiate(graphPrefab, new Vector3(position.x, position.y), Quaternion.identity);

		float leftBound = -(xDimension / 2);
		float upperBound = (yDimension / 2);

		int numOfXPos = Mathf.FloorToInt(xDimension / nodeSeperation) + 1;
		int numOfYPos = Mathf.FloorToInt(yDimension / nodeSeperation) + 1;

		Node[,] nodes = new Node[numOfXPos, numOfYPos];

		Debug.Log("Starting Generation");
		for (int i = 0; i < numOfXPos; i++)
		{
			float xPos = leftBound + (i * nodeSeperation);

			for (int j = 0; j < numOfYPos; j++)
			{
				float yPos = upperBound - (j * nodeSeperation);

				// Check for ground and obstacles
				if (IsPosValid(new Vector2(xPos, yPos)))
				{
					Debug.Log("Placing Waypoint");
					GameObject wpObj = Instantiate(wpObject, new Vector3(xPos, yPos), Quaternion.identity, graphObj.transform);
					nodes[i, j] = new Node(wpObj);
				}
				else
					nodes[i, j] = null;
			}
		}

		GenerateLinks(nodes, numOfXPos, numOfYPos);
		Graph graph = new Graph(nodes, xDimension, yDimension, graphObj);
		graphs.Add(graph);
		return graphs.Count - 1;
	}

	public void GenerateLink(HashSet<Vector2Int> tiles, Tilemap map, bool diagonal)
	{
		if (tiles.Count == 0)
			return;

		links_generated = false;
		nodeDictionary = new Dictionary<Vector2Int, Node>();

		// Set Up Nodes
		foreach (Vector2Int tile in tiles)
		{
			Node node = new Node(map.GetInstantiatedObject(new Vector3Int(tile.x, tile.y)));
			node.position = tile;
			nodeDictionary.Add(tile, node);
		}

		// Establish Neighbors
		foreach (Vector2Int tile in tiles)
		{
			Node node = nodeDictionary[tile];

			Node neighbor;

			// Check for right neighbor
			if (nodeDictionary.TryGetValue(new Vector2Int(tile.x + 1, tile.y), out neighbor))
				node.edgeList.Add(new Edge(node, neighbor));

			// Check for left neighbor
			if (nodeDictionary.TryGetValue(new Vector2Int(tile.x - 1, tile.y), out neighbor))
				node.edgeList.Add(new Edge(node, neighbor));

			// Check for bottom neighbor
			if (nodeDictionary.TryGetValue(new Vector2Int(tile.x, tile.y - 1), out neighbor))
				node.edgeList.Add(new Edge(node, neighbor));

			// Check for top neighbor
			if (nodeDictionary.TryGetValue(new Vector2Int(tile.x, tile.y + 1), out neighbor))
				node.edgeList.Add(new Edge(node, neighbor));
			
			if (diagonal)
			{
				// Check for top right neighbor
				if (nodeDictionary.TryGetValue(new Vector2Int(tile.x + 1, tile.y + 1), out neighbor))
					node.edgeList.Add(new Edge(node, neighbor));

				// Check for top left neighbor
				if (nodeDictionary.TryGetValue(new Vector2Int(tile.x - 1, tile.y + 1), out neighbor))
					node.edgeList.Add(new Edge(node, neighbor));

				// Check for bottom right neighbor
				if (nodeDictionary.TryGetValue(new Vector2Int(tile.x + 1, tile.y - 1), out neighbor))
					node.edgeList.Add(new Edge(node, neighbor));

				// Check for bottom left neighbor
				if (nodeDictionary.TryGetValue(new Vector2Int(tile.x - 1, tile.y - 1), out neighbor))
					node.edgeList.Add(new Edge(node, neighbor));
			}
		}

		tileMap = map;
		links_generated = true;
	}
	void GenerateLinks(Node[,] nodes, int numOfXPos, int numOfYPos)
	{
		if (numOfXPos == 0 || numOfYPos == 0) return;


		for (int i = 0; i < numOfXPos; i++)
		{
			for (int j = 0; j < numOfYPos; j++)
			{
				Node node = nodes[i, j];
				if (node != null)
				{
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
						if (i - 1 >= 0 && j - 1 >= 0 && nodes[i - 1, j - 1] != null)
							node.edgeList.Add(new Edge(node, nodes[i - 1, j - 1]));

						if (i + 1 < numOfXPos && j - 1 >= 0 && nodes[i + 1, j - 1] != null)
							node.edgeList.Add(new Edge(node, nodes[i + 1, j - 1]));

						if (i - 1 >= 0 && j + 1 < numOfYPos && nodes[i - 1, j + 1] != null)
							node.edgeList.Add(new Edge(node, nodes[i - 1, j + 1]));

						if (i + 1 < numOfXPos && j + 1 < numOfYPos && nodes[i + 1, j + 1] != null)
							node.edgeList.Add(new Edge(node, nodes[i + 1, j + 1]));
					}
				}

				 
				
			}
		}
	}

	/// <summary>
	/// Check that the provided position is a valid point to place a node
	/// </summary>
	/// <param stateName="position"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Delete the graph and its nodes at the corresponding index
	/// </summary>
	/// <param stateName="index"></param>
	public void DeleteWaypoints(int index)
	{
		Graph graph = graphs[index];
		foreach (Node node in graph.nodes)
		{
			if (node != null)
			{
				GameObject obj = node.getId();
				DestroyImmediate(obj);
			}
			
		}
		graph.nodes = null;
		DestroyImmediate(graph.obj);
		graphs.Remove(graphs[index]);
	}

	#endregion

	public Node GetNode(GameObject nodeObj)
	{
		Vector2Int pos = (Vector2Int)tileMap.WorldToCell(nodeObj.transform.position);
		if (nodeDictionary.TryGetValue(pos, out Node node))
			return node;

		return null;

	}
	
	#region Test Methods
	public void Test_Generate()
	{
		if (test_generated)
		{
			Test_Delete();
		}

		GenerateGrid(this.transform.position, test_length, test_width, test_seperationDist);
		test_generated = true;
	}

	public void Test_Delete()
	{
		if (!test_generated) return;
		if (graphs.Count > 0)
			DeleteWaypoints(0);
		graphs.Clear();
		test_generated = false;
	}
	#endregion

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
		Gizmos.DrawCube(this.transform.position, new Vector3(test_length, test_width, 1));
		Gizmos.color = Color.white;

	}

	private void OnDrawGizmosSelected()
	{
		if (test_generated && graphs.Count > 0)
		{
			Gizmos.color = Color.yellow;
			foreach (Node node in graphs[0].nodes)
			{
				if (node != null)
				{
					foreach (Edge edge in node.edgeList)
					{
						Vector3 start = edge.startNode.getId().transform.position;
						Vector3 end = edge.endNode.getId().transform.position;
						Gizmos.DrawLine(start, end);
					}
				}
				
			}
			Gizmos.color = Color.white;
		}

		if (links_generated && nodeDictionary.Count > 0)
		{
			Gizmos.color = Color.yellow;
			foreach (Node node in  nodeDictionary.Values)
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
