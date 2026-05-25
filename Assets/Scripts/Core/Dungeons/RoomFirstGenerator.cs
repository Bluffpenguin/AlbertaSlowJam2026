using System.Linq;

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

		using (UnityEngine.Pool.ListPool<IRoomPostProcessor>.Get(out var components)) {
			this.GetComponents(components);
			foreach (var room in _rooms.Values) {
				for (int i = 0; i < components.Count; i++) {
					components[i].ProcessRoom(room);
				}
			}
		}

		var walls = WallGenerator.CreateWalls(floor, _wallPainter);
		floor.UnionWith(walls);
		WallGenerator.CreateWalls(floor, _wallPainter);
	}

	private HashSet<Vector2Int> CreateSimpleRooms(List<RectInt> rooms)
	{
		var floor = new HashSet<Vector2Int>();

		foreach (var room in rooms) {
			var roomFloor = new HashSet<Vector2Int>();
			var roomBounds = PadRect(room, _offset);
			foreach (var position in roomBounds.allPositionsWithin) {
				roomFloor.Add(position);
			}
			var origin = Vector2Int.RoundToInt(roomBounds.center);
			_rooms.Add(origin, new RoomInfo(origin, roomFloor, 0));
			floor.UnionWith(roomFloor);
		}
		return floor;
	}

	private HashSet<Vector2Int> CreateRandomWalkRooms(List<RectInt> roomPositions)
	{
		HashSet<Vector2Int> floor = new();
		for (int i = 0; i < roomPositions.Count; i++) {
			var bounds = PadRect(roomPositions[i], _offset);
			var center = Vector2Int.RoundToInt(bounds.center);
			var roomFloor = GenerateRoom(center, _preset).Where(p => bounds.Contains(p)).ToHashSet();
			
			_rooms.Add(center, new RoomInfo(center, roomFloor, 0));
			floor.UnionWith(roomFloor);
		}
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
