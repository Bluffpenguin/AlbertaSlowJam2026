using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Pathfinder))]
public class RoomManager : MonoBehaviour
{
	Dictionary<Vector2Int, Node> nodeDictionary;
	Tilemap tileMap;
	bool links_generated = false;

	internal Pathfinder pf;


	private void Start()
	{
		pf = GetComponent<Pathfinder>();
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

	}
}
