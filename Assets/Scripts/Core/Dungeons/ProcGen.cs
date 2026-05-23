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

	public static List<RectInt> BinarySpacePartitioning(RectInt source, Vector2Int minSize)
	{
		var roomsQueue = new Queue<RectInt>(capacity: 1);
		roomsQueue.Enqueue(source);
		var rooms = new List<RectInt>();
		while (roomsQueue.Count > 0) {
			var room = roomsQueue.Dequeue();
			if (room.size.x < minSize.x || room.size.y < minSize.y) {
				continue;
			}

			if (Random.value < 0.5f) {
				if (room.size.y >= minSize.y * 2) {
					SplitHorizontal(room, minSize.y, roomsQueue);
				} else if (room.size.x >= minSize.x * 2) {
					SplitVertical(room, minSize.x, roomsQueue);
				} else {
					rooms.Add(room);
				}
			} else {
				if (room.size.x >= minSize.x * 2) {
					SplitVertical(room, minSize.x, roomsQueue);
				} else if (room.size.y >= minSize.y * 2) {
					SplitHorizontal(room, minSize.y, roomsQueue);
				} else {
					rooms.Add(room);
				}
			}
		}

		return rooms;
	}

	private static void SplitVertical(RectInt rect, int minWidth, Queue<RectInt> queue)
	{
		var split = Random.Range(1, rect.size.x);
		var a = new RectInt(rect.xMin, rect.xMin, split, rect.height);
		var b = new RectInt(rect.xMin + split, rect.yMin, rect.width - split, rect.height);
		if (a.width >= minWidth)
			queue.Enqueue(a);
		if (b.width >= minWidth)
			queue.Enqueue(b);
	}

	private static void SplitHorizontal(RectInt rect, int minHeight, Queue<RectInt> queue)
	{
		var split = Random.Range(1, rect.height);
		var a = new RectInt(rect.xMin, rect.yMin, rect.width, split);
		var b = new RectInt(rect.xMin, rect.yMin + split, rect.width, rect.height - split);
		if (a.height >= minHeight)
			queue.Enqueue(a);
		if (b.height >= minHeight)
			queue.Enqueue(b);
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
