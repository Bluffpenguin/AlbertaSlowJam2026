using UnityEngine;

public class Pathfinder : MonoBehaviour
{
	public List<Node> pathList = new List<Node>();

	[SerializeField] LayerMask obstructions;
	[SerializeField] RoomManager rm;

	public bool AStar(Node start, Node end)
	{
		if (start == null || end == null) return false;


		if (start.getId() == end.getId())
		{
			pathList.Clear();
			return false;
		}

		List<Node> open = new List<Node>();
		List<Node> closed = new List<Node>();
		float tentative_g_score = 0;
		bool tentative_is_better;

		start.g = 0;
		start.h = distance(start, end);
		start.f = start.h;

		open.Add(start);
		while (open.Count > 0)
		{
			int i = lowestF(open);
			Node thisNode = open[i];

			
			if (thisNode.getId() == end.getId())
			{
				ReconstructPath(start, end);
				return true;
			}

			open.RemoveAt(i);
			closed.Add(thisNode);
			Node neighbour;
			foreach (Edge e in thisNode.edgeList)
			{
				neighbour = e.endNode;

				if (closed.IndexOf(neighbour) > -1)
					continue;

				tentative_g_score = thisNode.g + distance(thisNode, neighbour);
				if (open.IndexOf(neighbour) == -1)
				{
					open.Add(neighbour);
					tentative_is_better = true;
				}
				else if (tentative_g_score < neighbour.g)
				{
					tentative_is_better = true;
				}
				else tentative_is_better = false;

				if (tentative_is_better)
				{
					neighbour.cameFrom = thisNode;
					neighbour.g = tentative_g_score;
					neighbour.h = distance(thisNode, end);
					neighbour.f = neighbour.g + neighbour.h;
				}
			}
		}
		return false;
	}

	public void ReconstructPath(Node startId, Node endId)
	{
		pathList.Clear();
		pathList.Add(endId);

		var p = endId.cameFrom;
		while (p != startId && p != null)
		{
			pathList.Insert(0, p);
			p = p.cameFrom;
		}
		pathList.Insert(0, startId);
	}

	float distance(Node a, Node b)
	{
		return (Vector3.SqrMagnitude(a.getId().transform.position - b.getId().transform.position));
	}

	int lowestF(List<Node> l)
	{
		float lowestf = 0;
		int count = 0;
		int iteratorCount = 0;

		lowestf = l[0].f;

		for (int i = 1; i < l.Count; i++)
		{
			if (l[i].f < lowestf)
			{
				lowestf = l[i].f;
				iteratorCount = count;
			}
			count++;
		}
		return iteratorCount;
	}

	public List<Node> ShavePath(List<Node> path)
	{
		if (path.Count <= 2) return path;
		List<Node> optimizedPath = new List<Node>();

		int currentNode = 0;
		int endNode = path.Count - 1;

		optimizedPath.Add(path[currentNode]);

		int prevention = 0;

		while (currentNode < endNode && prevention != 99)
		{
			Vector3 currPos = rm.navTileMap.CellToWorld((Vector3Int)path[currentNode].position);
			bool foundShortcut = false;

			for (int i = endNode; i > currentNode; i--)
			{
				Vector3 checkPos = rm.navTileMap.CellToWorld((Vector3Int)path[i].position);
				//Vector2 direction = (checkPos - currPos).normalized;
				//float distance = Vector2.Distance(currPos, checkPos);

				
				if (!Physics2D.Linecast(currPos, checkPos, obstructions))
				{
					optimizedPath.Add(path[i]);
					currentNode = i;
					foundShortcut = true;
					break;
				}
				
				
				Debug.Log("Detected wall");
			}
			prevention++;

			if (!foundShortcut)
			{
				currentNode++;
				optimizedPath.Add(path[currentNode]);
			}
		}

		if (prevention != 99)
			return optimizedPath;
		else
			Debug.Log("Failed to find smooth path");
		return path;
	}
}
