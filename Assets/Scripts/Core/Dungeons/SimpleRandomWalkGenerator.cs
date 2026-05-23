using System.Linq;

public class SimpleRandomWalkGenerator : MonoBehaviour
{
	public Vector2Int startPosition = Vector2Int.zero;
	public int iterations = 10;
	public int length = 10;
	[Tooltip("Rooms will be larger and more cave-like.")]
	public bool largeRooms = true;

	[SerializeField]
	private TilePainter _painter;

	public void Generate()
	{
		HashSet<Vector2Int> floor = RunRandomWalk();
		_painter.PaintFloorTiles(floor, true);
	}

	protected HashSet<Vector2Int> RunRandomWalk()
	{
		var currentPosition = startPosition;
		var floor = new HashSet<Vector2Int>();
		for (int i = 0; i < iterations; i++) {
			var path = ProcGen.SimpleRandomWalk(currentPosition, length);
			floor.UnionWith(path);
			if (largeRooms) {
				currentPosition = floor.ElementAt(Random.Range(0, floor.Count));
			}
		}
		return floor;
	}
}
