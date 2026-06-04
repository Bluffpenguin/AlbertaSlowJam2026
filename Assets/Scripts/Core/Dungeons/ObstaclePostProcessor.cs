using System.Linq;

public class ObstaclePostProcessor : SingleTilePainter, IRoomPostProcessor
{
	[SerializeField] protected int _processingOrder;
	[SerializeField, Min(0)] protected int _maxCount = 1;
	[SerializeField] protected AnimationCurve _countScalingByDistance = new(new Keyframe(0, 1), new Keyframe(1, 1));
	[SerializeField] protected ObstacleData _data;

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
		var size = _data.Dimensions;

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
			base.PaintTile(position);
		}

		if (_data.Impassable) {
			room.Tiles.ExceptWith(area);
		}
	}
}
