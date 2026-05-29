using System.Linq;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Pathfinder))]
public class RoomManager : MonoBehaviour
{
	Dictionary<Vector2Int, Node> nodeDictionary;
	internal Tilemap tileMap;
	bool links_generated = false;

	internal Pathfinder pf;
	[System.Serializable] struct Patrol { public List<Node> patrolPath; public Node guardSpot; }
	[SerializeField] private Patrol[] patrolList;
	private int actualEnemiesCount = 0;
	public int expectedEnemiesCount = 0;


	private void Start()
	{
		pf = GetComponent<Pathfinder>();
		patrolList = new Patrol[expectedEnemiesCount];
		for (int i = 0; i < expectedEnemiesCount; i++)
		{
			patrolList[i].patrolPath = null;
			patrolList[i].guardSpot = null;
		}
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

	public Node GetNode(GameObject nodeObj)
	{
		Vector2Int pos = (Vector2Int)tileMap.WorldToCell(nodeObj.transform.position);
		if (nodeDictionary.TryGetValue(pos, out Node node))
			return node;

		return null;

	}

	public Node GetNode(Vector2Int pos)
	{
		if (nodeDictionary.TryGetValue(pos, out Node node))
			return node;

		return null;

	}

	public Node GetRandomFleePosition(Transform retreatFrom, float minDistance)
	{
		List<Vector2Int> roomPositions = nodeDictionary.Keys.ToList();
		Vector2Int retreatFromPos = (Vector2Int)tileMap.WorldToCell(retreatFrom.position);

		int fallback = 0;
		Vector2Int currPosition = retreatFromPos;
		while (Vector2Int.Distance(currPosition, retreatFromPos) < minDistance && fallback < 25)
		{
			int rand = Random.Range(0, roomPositions.Count);
			currPosition = roomPositions[rand];
			fallback++;
		}

		return GetNode(currPosition);
		
	}

	public void AddPatrol(List<Node> patrol, int enemyId)
	{
		if (actualEnemiesCount == expectedEnemiesCount)
		{
			Debug.Log("This room cant support that many enemies");
			return;
		}

		patrolList[enemyId].patrolPath = patrol;
		actualEnemiesCount++;
	}

	public void AddGuard(Node guardSpot, int enemyId)
	{
		if (actualEnemiesCount == expectedEnemiesCount)
		{
			Debug.Log("This room cant support that many enemies");
			return;
		}
		patrolList[enemyId].guardSpot = guardSpot;
		actualEnemiesCount++;
	}

	public List<Node> GetPatrol(int index)
	{
		return patrolList[index].patrolPath;
	}

	public Node GetGuardSpot(int index)
	{
		return patrolList[index].guardSpot;
	}


	private void OnDrawGizmosSelected()
	{

		if (links_generated && nodeDictionary.Count > 0)
		{
			Gizmos.color = Color.yellow;
			foreach (Node node in nodeDictionary.Values)
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

		if (actualEnemiesCount > 0)
		{
			foreach (Patrol patrol in patrolList)
			{
				
				if (patrol.patrolPath != null)
				{
					Gizmos.color = new Color(0f, 0f, 1f, 0.4f);
					for (int i = 0; i < patrol.patrolPath.Count; i++)
					{
						if (i + 1 < patrol.patrolPath.Count)
						{
							// Draw lines between points
							Gizmos.DrawLine(patrol.patrolPath[i].getId().transform.position,
											patrol.patrolPath[i+1].getId().transform.position);
						}

						Gizmos.DrawSphere(patrol.patrolPath[i].getId().transform.position, tileMap.cellSize.x / 5);
					}
				}
				else
				{
					Gizmos.color = new Color(1f, 0.27f, 0f, 0.4f);
					Gizmos.DrawSphere(patrol.guardSpot.getId().transform.position, tileMap.cellSize.x / 5);
				}
			}
			Gizmos.color = Color.white;
		}
		

	}
}
