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

	public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int length)
	{
		var corridor = new List<Vector2Int>() { startPosition };
		var direction = Direction2D.GetRandomDirection();
		var currentPosition = startPosition;
		for (int i = 0; i < length; i++) {
			currentPosition += direction;
			corridor.Add(currentPosition);
		}
		return corridor;
	}
}

public static class Direction2D
{
	public static readonly IReadOnlyList<Vector2Int> cardinals = new Vector2Int[4] {
		Vector2Int.up,
		Vector2Int.left,
		Vector2Int.down,
		Vector2Int.right
	};

	public static readonly IReadOnlyList<Vector2Int> ordinals = new Vector2Int[4] {
		Vector2Int.up + Vector2Int.left,
		Vector2Int.down + Vector2Int.left,
		Vector2Int.down + Vector2Int.right,
		Vector2Int.up + Vector2Int.right
	};

	public static Vector2Int GetRandomDirection()
	{
		return cardinals[Random.Range(0, cardinals.Count)];
	}
}
