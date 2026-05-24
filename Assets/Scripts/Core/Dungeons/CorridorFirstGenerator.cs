using System.Linq;

public class CorridorFirstGenerator : SimpleRandomWalkGenerator
{
	[SerializeField]
	private int _corridorLength = 14, _corridorCount = 5;
	[SerializeField]
	[Range(0, 1)]
	private float _roomPercentage = 0.8f;

	public override void Generate()
	{
		Clear();
		var floor = CreateCorridors(out var potentialRooms);
		var rooms = GenerateRooms(floor, potentialRooms);
		floor.UnionWith(rooms);
		_floorPainter.PaintTiles(floor);
		var walls = WallGenerator.CreateWalls(floor, _wallPainter);
		floor.UnionWith(walls);
		WallGenerator.CreateWalls(floor, _wallPainter);
	}

	private HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> floor, HashSet<Vector2Int> potentialRooms)
	{
		var rooms = new HashSet<Vector2Int>();
		int roomCount = Mathf.RoundToInt(potentialRooms.Count * _roomPercentage);

		var roomPositions = potentialRooms.OrderBy(x => Guid.NewGuid())
			// Sorts by an arbitrary value to randomize the order
			.Take(roomCount)
			.ToList();

		foreach (var position in roomPositions) {
			var room = RunRandomWalk(position, _preset);
			rooms.UnionWith(room);
		}

		if (_roomPercentage < 1) {
			var deadEnds = from position in floor
						   where !rooms.Contains(position)
						   let neighbours = Direction2D.cardinals.Count(dir => floor.Contains(position + dir))
						   where neighbours == 1
						   select position;

			foreach (var position in deadEnds) {
				var room = RunRandomWalk(position, _preset);
				rooms.UnionWith(room);
			}
		}

		return rooms;
	}

	private HashSet<Vector2Int> CreateCorridors(out HashSet<Vector2Int> potentialRooms)
	{
		var floor = new HashSet<Vector2Int>();
		potentialRooms = new HashSet<Vector2Int>() { _startPosition };
		var currentPosition = _startPosition;
		for (int i = 0; i < _corridorCount; i++) {
			var corridor = ProcGen.RandomWalkCorridor(currentPosition, _corridorLength);
			currentPosition = corridor[^1];
			potentialRooms.Add(currentPosition);
			floor.UnionWith(corridor);
		}
		return floor;
	}
}
