using System.Linq;
using UnityEngine.Pool;

public class CorridorFirstGenerator : SimpleRandomWalkGenerator
{
	[Space]
	[SerializeField] private TilePainter _corridorPainter;
	[SerializeField] private ProcGenCorridorData _corridorData;
	[Space]
	[SerializeField]
	[Range(0, 1)] private float _roomPercentage = 0.8f;

	private readonly Dictionary<Vector2Int, RoomInfo> _rooms = new();

	public override void Clear()
	{
		_corridorPainter.Clear();
		base.Clear();
		_rooms.Clear();
	}

	public override void Generate()
	{
		Clear();
		var floor = CreateCorridors(out var potentialRooms);
		_corridorPainter.PaintTiles(floor);
		var rooms = GenerateRooms(floor, potentialRooms);
		_floorPainter.PaintTiles(rooms);
		floor.UnionWith(rooms);
		WallGenerator.CreateWalls(floor, _wallPainter, _wallData);
		base.ApplyPostProcessing(_rooms.Values);
	}

	private HashSet<Vector2Int> CreateCorridors(out HashSet<Vector2Int> potentialRooms)
	{
		var floor = new HashSet<Vector2Int>();
		potentialRooms = new HashSet<Vector2Int>() { _startPosition };
		var currentPosition = _startPosition;
		for (int i = 0; i < _corridorData.Count; i++) {
			var corridor = ProcGen.RandomWalk(currentPosition, walkLength: 1, _corridorData.Length)
				.ToArray();
			currentPosition = corridor[^1];
			potentialRooms.Add(currentPosition);
			floor.UnionWith(corridor);
		}

		var directions = Direction2D.Type.Cardinal;
		if (!_corridorData.RoundedCorners) {
			directions |= Direction2D.Type.Ordinal;
		}
		WallGenerator.ExtendFloorInDirections(floor, _corridorData.Width, Direction2D.GetDirections(directions));
		return floor;
	}

	private HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> floor, HashSet<Vector2Int> potentialRooms)
	{
		var rooms = new HashSet<Vector2Int>();
		int roomCount = Mathf.RoundToInt(potentialRooms.Count * _roomPercentage);

		var roomPositions = potentialRooms.OrderBy(x => Guid.NewGuid())
			// Sorts by an arbitrary value to randomize the order
			.Take(roomCount)
			.ToList();

		if (!roomPositions.Contains(_startPosition))
			roomPositions.Add(_startPosition);

		var roomsDict = DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Get();
		foreach (var position in roomPositions) {
			var room = GenerateRoom(position, _roomData);
			roomsDict.Add(position, room);
			rooms.UnionWith(room);
		}

		if (_roomPercentage < 1) {
			foreach (var position in FindDeadEnds(floor, rooms)) {
				var room = GenerateRoom(position, _roomData);
				roomsDict.Add(position, room);
				rooms.UnionWith(room);
			}
		}

		var distances = roomsDict.Keys.ToDictionary(static p => p, p => Vector2Int.Distance(p, _startPosition));
		float min = distances.Values.Min();
		float max = distances.Values.Max();

		foreach (var key in roomsDict.Keys) {
			var info = new RoomInfo(key, roomsDict[key], Mathf.InverseLerp(min, max, distances[key]));
			_rooms.Add(key, info);
		}
		DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Release(roomsDict);

		return rooms;
	}

	private IEnumerable<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floor, HashSet<Vector2Int> rooms)
	{
		return
			from pos in floor
			where !rooms.Contains(pos)
			let neighbours = Direction2D.cardinals.Count(dir => floor.Contains((dir * _corridorData.Width) + pos))
			where neighbours is 1
			select pos;
	}
}
