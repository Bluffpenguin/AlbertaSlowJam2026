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
		var floor = RunRandomWalk();
		_floorPainter.PaintTiles(floor);
		WallGenerator.CreateWalls(floor, _wallPainter);
	}

	protected HashSet<Vector2Int> RunRandomWalk()
	{
		Debug.Assert(_preset != null);
		var currentPosition = _startPosition;
		var floor = new HashSet<Vector2Int>();
		for (int i = 0; i < _preset.Iterations; i++) {
			var path = ProcGen.SimpleRandomWalk(currentPosition, _preset.Length);
			floor.UnionWith(path);
			if (_preset.LargeRooms) {
				currentPosition = floor.ElementAt(Random.Range(0, floor.Count));
			}
		}
		return floor;
	}
}
