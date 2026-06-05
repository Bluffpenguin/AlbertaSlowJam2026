using System.Linq;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class SimpleRandomWalkGenerator : BaseDungeonGenerator
{
	[SerializeField] protected TilePainter _floorPainter;
	[FormerlySerializedAs("_preset")]
	[SerializeField] protected ProcGenRoomData _roomData;
	[Space]
	[SerializeField] protected TilePainter _wallPainter;
	[SerializeField] protected ProcGenWallData _wallData;

	public override void Clear()
	{
		_floorPainter.Clear();
		_wallPainter.Clear();

		using (ListPool<IRoomPostProcessor>.Get(out var components)) {
			this.GetComponentsInChildren(components);
			components.Sort();
			for (int i = 0; i < components.Count; i++) {
				components[i].Clear();
			}
		}
	}

	public override void Generate()
	{
		Clear();
		var floor = GenerateRoom(_startPosition, _roomData);
		_floorPainter.PaintTiles(floor);
		WallGenerator.CreateWalls(floor, _wallPainter, _wallData);

		var roomInfo = new RoomInfo(_startPosition, floor, 0);
		this.ApplyPostProcessing(roomInfo);
	}

	protected void ApplyPostProcessing(RoomInfo room)
	{
		var components = ListPool<IRoomPostProcessor>.Get();
		this.GetComponentsInChildren(components);
		components.Sort();
		for (int i = 0; i < components.Count; i++) {
			var processor = components[i];
			if (processor == null || processor.enabled is false)
				continue;
			try {
				processor.ProcessRoom(room);
			}
			catch (Exception ex) {
				Debug.LogWarning($"An error occurred while applying processor {processor}.\n{ex}", this);
			}
		}
		ListPool<IRoomPostProcessor>.Release(components);
	}

	protected void ApplyPostProcessing(IEnumerable<RoomInfo> rooms)
	{
		var components = ListPool<IRoomPostProcessor>.Get();
		this.GetComponentsInChildren(components);
		components.Sort();
		foreach (var room in rooms) {
			for (int i = 0; i < components.Count; i++) {
				if (components[i].enabled)
					components[i].ProcessRoom(room);
			}
		}
		ListPool<IRoomPostProcessor>.Release(components);
	}

	protected static HashSet<Vector2Int> GenerateRoom(Vector2Int startPosition, ProcGenRoomData data)
	{
		if (data == null)
			throw new ArgumentNullException(nameof(data));

		var min = startPosition - Vector2Int.RoundToInt((Vector2)data.MaxRoomSize / 2f);
		var roomBounds = new RectInt(min, data.MaxRoomSize);

		var currentPosition = startPosition;
		var floor = new HashSet<Vector2Int>();
		for (int i = 0; i < data.Iterations; i++) {
			var path = ProcGen.RandomWalk(currentPosition, data.Length);
			floor.UnionWith(path.Where(roomBounds.Contains));
			if (data.LargeRooms) {
				currentPosition = floor.ElementAt(Random.Range(0, floor.Count));
			}
		}
		return floor;
	}
}
