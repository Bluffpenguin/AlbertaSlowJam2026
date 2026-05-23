public static class ProcGen
{
	public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int length)
	{
		var path = new HashSet<Vector2Int> { startPosition };
		var previousPosition = startPosition;
		for (int i = 0; i < length; i++) {
			var newPosition = previousPosition + Direction2D.GetRandomDirection();
			path.Add(newPosition);
			previousPosition = newPosition;
		}
		return path;
	}
}

public static class Direction2D
{
	public static IReadOnlyList<Vector2Int> cardinalDirections = new Vector2Int[4] {
		Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
	};

	public static Vector2Int GetRandomDirection()
	{
		return cardinalDirections[Random.Range(0, cardinalDirections.Count)];
	}
}
