using UnityEngine;

[System.Serializable]
public class Node 
{
	public List<Edge> edgeList = new List<Edge>();
	GameObject id;
	public Vector2Int position;

	public float f, g, h;
	public Node cameFrom;

	public Node(GameObject i)
	{
		id = i;
	}

	public GameObject getId()
	{
		return id;
	}

	public void setId(GameObject obj)
	{
		id = obj;
	}
}

public class Edge
{
	public Node startNode;
	public Node endNode;

	public Edge(Node from, Node to)
	{
		startNode = from;
		endNode = to;
	}
}
