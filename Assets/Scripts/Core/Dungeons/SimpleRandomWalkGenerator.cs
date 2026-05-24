using System.Linq;

public class SimpleRandomWalkGenerator : BaseDungeonGenerator
{
	[SerializeField] protected TilePainter _floorPainter;
	[SerializeField] protected TilePainter _wallPainter;
	[SerializeField] protected RandomWalkData _preset = null;

	public override void Clear()
	{
		_floorPainter.Clear();
		_wallPainter.Clear();
	}

	public override void Generate()
	{
		Clear();
		var floor = RunRandomWalk(_startPosition, _preset);
		_floorPainter.PaintTiles(floor);
		WallGenerator.CreateWalls(floor, _wallPainter);
	}

	protected static HashSet<Vector2Int> RunRandomWalk(Vector2Int startPosition, RandomWalkData data)
	{
		if (data == null)
			throw new ArgumentNullException(nameof(data));

		var currentPosition = startPosition;
		var floor = new HashSet<Vector2Int>();
		for (int i = 0; i < data.Iterations; i++) {
			var path = ProcGen.RandomWalk(currentPosition, data.Length);
			floor.UnionWith(path);
			if (data.LargeRooms) {
				currentPosition = floor.ElementAt(Random.Range(0, floor.Count));
			}
		}
		return floor;
	}
}
