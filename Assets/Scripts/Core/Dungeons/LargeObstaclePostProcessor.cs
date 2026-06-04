using System.Linq;
using UnityEngine.Tilemaps;

[Obsolete("Use Obstacle Post Processor instead.")]
public class LargeObstaclePostProcessor : SingleTilePainter, IRoomPostProcessor
{
	[Tooltip("The tile used only for collisions")]
	[SerializeField] protected TileBase _dummyTile;
	[SerializeField] protected int _processingOrder;
	[SerializeField, Min(0)] protected int _maxCount = 1;
	[SerializeField] protected AnimationCurve _countScalingByDistance = new(new Keyframe(0, 1), new Keyframe(1, 1));
	[SerializeField] protected LargeObstacleData _data;

	public int Order => _processingOrder;

	public virtual void ProcessRoom(RoomInfo room)
	{
		int count = Mathf.RoundToInt(_countScalingByDistance.Evaluate(room.Distance) * _maxCount);
		for (int i = 0; i < count; i++) {
			PlaceObstacle(room);
		}
	}

	public virtual void PlaceObstacle(RoomInfo room)
	{
		var size = _data.Size;

		if (_data.AllowMirror && Random.value < 0.5f) {
			(size.x, size.y) = (size.y, size.x);
		}

		Vector2Int[] directions = Direction2D.GetDirections(_data.CheckDirections);
		var origin = room.Tiles.ElementAt(Random.Range(0, room.Tiles.Count));

		var dimensions = new RectInt(origin, size);
		bool placeable = true;

		var area = dimensions.Area().ToArray();
		for (int i = 0; i < area.Length; i++) {
			Vector2Int position = area[i];
			placeable &= room.Tiles.Contains(position);
		}

		if (!placeable)
			return;

		int wallCount = 0, floorCount = 0;
		for (int i = 0; i < directions.Length; i++) {
			Vector2Int direction = directions[i];
			var tiles = (from position in dimensions.Perimeter()
						 let tile = direction + position
						 where !dimensions.Contains(tile)
						 select tile).ToArray();

			if (tiles.All(t => room.Tiles.Contains(t)))
				floorCount++;
			else
				wallCount++;
		}

		if (_data.Requirements.HasFlag(ObstacleData.SpaceRequirements.NextToWall)) {
			placeable &= wallCount < directions.Length;
		}

		if (_data.Requirements.HasFlag(ObstacleData.SpaceRequirements.OpenSpace)) {
			placeable &= floorCount == directions.Length;
		}

		if (!placeable)
			return;

		foreach (var position in area) {
			if (true || position == origin) {
				base.PaintTile(position);
			} else {
				PaintTile(position, _tilemap, _dummyTile);
			}
		}

		if (_data.Impassable) {
			room.Tiles.ExceptWith(area);
		}
	}
}

public static class RectUtils
{
	public static IEnumerable<Vector2Int> Area(this RectInt rect)
	{
		for (int y = rect.yMin; y < rect.yMax; y++) {
			for (int x = rect.xMin; x < rect.xMax; x++) {
				yield return new Vector2Int(x, y);
			}
		}
	}

	public static IEnumerable<Vector2Int> Perimeter(this RectInt rect)
	{
		for (int y = rect.yMin; y < rect.yMax; y++) {
			for (int x = rect.xMin; x < rect.xMax; x++) {
				if (x == rect.xMin || x == rect.xMax || y == rect.yMin || y == rect.yMax) {
					yield return new Vector2Int(x, y);
				}
			}
		}
	}

	public static IEnumerable<Vector2Int> Row(this RectInt rect, int y)
	{
		for (int x = rect.xMin; x < rect.xMax; x++) {
			yield return new Vector2Int(x, y);
		}
	}

	public static IEnumerable<Vector2Int> Column(this RectInt rect, int x)
	{
		for (int y = rect.yMin; y < rect.yMax; y++) {
			yield return new Vector2Int(x, y);
		}
	}

	public static RectInt PadInward(this RectInt rect, int padding)
	{
		return new RectInt() {
			xMin = rect.xMin + padding,
			yMin = rect.yMin + padding,
			xMax = rect.xMax - padding,
			yMax = rect.yMax - padding
		};
	}

	public static RectInt PadOutward(this RectInt rect, int left, int bottom, int right, int top)
		=> PadInward(rect, -left, -bottom, -right, -top);
	public static RectInt PadInward(this RectInt rect, int left, int bottom, int right, int top)
	{
		return new RectInt() {
			xMin = rect.xMin + left,
			yMin = rect.yMin + bottom,
			xMax = rect.xMax - right,
			yMax = rect.yMax - top
		};
	}
}
