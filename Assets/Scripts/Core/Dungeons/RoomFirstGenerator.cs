using System.Linq;
using UnityEngine.Pool;

public class RoomFirstGenerator : SimpleRandomWalkGenerator
{
	[SerializeField]
	private Vector2Int _dungeonSize;
	[SerializeField]
	[Min(1)]
	private Vector2Int _minRoomSize = Vector2Int.one;
	[SerializeField]
	[Min(0)] private int _offset;
	[SerializeField]
	private bool _useRandomWalk = false;

	private readonly Dictionary<Vector2Int, RoomInfo> _rooms = new();

	public override void Generate()
	{
		Clear();
		CreateRooms();
	}

	public override void Clear()
	{
		base.Clear();
		_rooms.Clear();
	}

	private void CreateRooms()
	{
		var rooms = ProcGen.BinarySpacePartitioning(new RectInt(_startPosition - (_dungeonSize / 2), _dungeonSize), _minRoomSize);

		var floor = _useRandomWalk ? CreateRandomWalkRooms(rooms) : CreateSimpleRooms(rooms);

		var roomCenters = rooms.Select(room => room.center).Select(Vector2Int.RoundToInt).ToList();
		var corridors = ConnectRooms(roomCenters);
		floor.UnionWith(corridors);
		_floorPainter.PaintTiles(floor);
		WallGenerator.CreateWalls(floor, _wallPainter, _wallThickness);
		base.ApplyPostProcessing(_rooms.Values);
	}

	private HashSet<Vector2Int> CreateSimpleRooms(List<RectInt> rooms)
	{
		var floor = new HashSet<Vector2Int>();

		var roomsDict = DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Get();
		foreach (var room in rooms) {
			var roomFloor = new HashSet<Vector2Int>();
			var roomBounds = PadRect(room, _offset);
			foreach (var position in roomBounds.allPositionsWithin) {
				roomFloor.Add(position);
			}
			var origin = Vector2Int.RoundToInt(roomBounds.center);
			roomsDict.Add(origin, roomFloor);
			floor.UnionWith(roomFloor);
		}

		var distances = roomsDict.Keys.ToDictionary(static p => p, p => Vector2Int.Distance(p, _startPosition));
		float min = distances.Values.Min();
		float max = distances.Values.Max();

		foreach (var key in roomsDict.Keys) {
			var info = new RoomInfo(key, roomsDict[key], Mathf.InverseLerp(min, max, distances[key]));
			_rooms.Add(key, info);
		}
		DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Release(roomsDict);
		return floor;
	}

	private HashSet<Vector2Int> CreateRandomWalkRooms(List<RectInt> roomPositions)
	{
		HashSet<Vector2Int> floor = new();

		var roomsDict = DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Get();
		for (int i = 0; i < roomPositions.Count; i++) {
			var bounds = PadRect(roomPositions[i], _offset);
			var center = Vector2Int.RoundToInt(bounds.center);
			var roomFloor = GenerateRoom(center, _preset).Where(p => bounds.Contains(p)).ToHashSet();
			
			roomsDict.TryAdd(center, roomFloor);
			floor.UnionWith(roomFloor);
		}

		var distances = roomsDict.Keys.ToDictionary(static p => p, p => Vector2Int.Distance(p, _startPosition));
		float min = distances.Values.Min();
		float max = distances.Values.Max();

		foreach (var key in roomsDict.Keys) {
			var info = new RoomInfo(key, roomsDict[key], Mathf.InverseLerp(min, max, distances[key]));
			_rooms.Add(key, info);
		}
		DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Release(roomsDict);

		return floor;
	}

	private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> rooms)
	{
		var corridors = new HashSet<Vector2Int>();

		var current = rooms[Random.Range(0, rooms.Count)];
		rooms.Remove(current);
		while (rooms.Count > 0) {
			var closest = FindNearestPoint(current, rooms);
			rooms.Remove(closest);
			HashSet<Vector2Int> corridor = CreateCorridor(current, closest);
			corridors.UnionWith(corridor);
			current = closest;
		}
		return corridors;
	}

	private HashSet<Vector2Int> CreateCorridor(Vector2Int current, Vector2Int destination)
	{
		var corridor = new HashSet<Vector2Int>() { current };
		var position = current;
		while (position.y != destination.y) {
			position.y += destination.y.CompareTo(position.y);
			corridor.Add(position);
		}
		while (position.x != destination.x) {
			position.x += destination.x.CompareTo(position.x);
			corridor.Add(position);
		}
		return corridor;
	}

	private static Vector2Int FindNearestPoint(Vector2Int position, IEnumerable<Vector2Int> points)
	{
		float min = float.MaxValue;
		Vector2Int result = default;
		foreach (var point in points) {
			var distance = Vector2Int.Distance(position, point);
			if (distance < min) {
				min = distance;
				result = point;
			}
		}
		return result;
	}

	private static RectInt PadRect(RectInt rect, int padding)
	{
		return new RectInt() {
			xMin = rect.xMin + padding,
			yMin = rect.yMin + padding,
			xMax = rect.xMax - padding,
			yMax = rect.yMax - padding
		};
	}
}
