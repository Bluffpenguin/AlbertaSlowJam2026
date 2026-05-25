using System.Linq;

public static class ProcGen
{
	public static IEnumerable<Vector2Int> RandomWalk(Vector2Int startPosition, int walkLength, int stepLength = 1)
	{
		yield return startPosition;
		var previousPosition = startPosition;
		for (int i = 0; i < walkLength; i++) {
			var direction = Direction2D.GetRandomDirection();
			for (int j = 0; j < stepLength; j++) {
				yield return previousPosition += direction;
			}
		}
	}

	[Obsolete]
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

	[Obsolete]
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

	public static readonly IReadOnlyList<Vector2Int> compass = new Vector2Int[8] {
		cardinals[0], ordinals[0],
		cardinals[1], ordinals[1],
		cardinals[2], ordinals[2],
		cardinals[3], ordinals[3]
	};

	[Flags]
	public enum Type { None = 0, Cardinal = 1, Ordinal = 2 }
	public enum Cardinal { North = 0, East = 1, South = 2, West = 3 }
	public enum Ordinal { NorthEast = 0, SouthEast = 1, SouthWest = 2, NorthWest = 3 }

	public static Vector2Int GetRandomDirection(Type type = Type.Cardinal)
	{
		return type switch {
			Type.Cardinal => cardinals[Random.Range(0, cardinals.Count)],
			Type.Ordinal => ordinals[Random.Range(0, ordinals.Count)],
			Type.Cardinal | Type.Ordinal => compass[Random.Range(0, compass.Count)],
			_ => Vector2Int.zero,
		};
	}

	public static Vector2Int GetDirection(object value) => value switch {
		Cardinal cardinal => cardinals[(int)cardinal],
		Ordinal ordinal => ordinals[(int)ordinal],
		_ => throw new ArgumentException("Value is not a direction.", nameof(value)),
	};

	public static Vector2Int[] GetDirections(Direction2D.Type type)
	{
		return type switch {
			Type.None => Array.Empty<Vector2Int>(),
			Type.Cardinal => cardinals.ToArray(),
			Type.Ordinal => ordinals.ToArray(),
			_ when type.HasFlag(Type.Cardinal | Type.Ordinal) => compass.ToArray(),
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};
	}
}
