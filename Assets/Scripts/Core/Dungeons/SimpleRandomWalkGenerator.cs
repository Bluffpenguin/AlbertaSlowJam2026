using System.Linq;
using UnityEngine.Tilemaps;

public class SimpleRandomWalkGenerator : BaseDungeonGenerator
{
	[SerializeField] protected TilePainter _floorPainter;
	[SerializeField] protected TilePainter _wallPainter;
	[SerializeField] protected RandomWalkData _preset = null;

	public override void Clear()
	{
		_floorPainter.Clear();
		_wallPainter.Clear();

		using (UnityEngine.Pool.ListPool<IRoomPostProcessor>.Get(out var components))
		{
			this.GetComponents(components);
			
			for (int i = 0; i < components.Count; i++)
			{
				components[i].Clear();

			}
			
		}
	}

	public override void Generate()
	{
		Clear();
		var floor = GenerateRoom(_startPosition, _preset);
		_floorPainter.PaintTiles(floor);
		//_pathPainter.PaintTiles(floor);
		WallGenerator.CreateWalls(floor, _wallPainter);
		//_pf.GenerateLink(floor, _pathPainter.Tilemap, false);
	}

	protected static HashSet<Vector2Int> GenerateRoom(Vector2Int startPosition, RandomWalkData data)
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
