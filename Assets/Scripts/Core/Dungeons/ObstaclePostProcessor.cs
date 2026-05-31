using System.Linq;
using System.Xml.Schema;

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
		Vector2Int[] directions = Direction2D.GetDirections(_data.CheckDirections);
		var position = room.Tiles.ElementAt(Random.Range(0, room.Tiles.Count));

		bool placeable = true;
		if (_data.Requirements.HasFlag(ObstacleData.SpaceRequirements.NextToWall)) {
			placeable &= directions.Count(d => !room.Tiles.Contains(position + d)) < directions.Length;
		}

		if (_data.Requirements.HasFlag(ObstacleData.SpaceRequirements.OpenSpace)) {
			placeable &= directions.All(d => room.Tiles.Contains(position + d));
		}

		if (placeable) {
			base.PaintTile(position);
		}

		if (_data.Impassable) {
			room.Tiles.Remove(position);
		}
	}
}
