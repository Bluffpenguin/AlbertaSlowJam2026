using System.Linq;

public class MultiObstaclePostProcessor : ObstaclePostProcessor
{
	[SerializeField] protected MultiObstacleData[] _table;

	public override void PlaceObstacle(RoomInfo room, Vector2Int[] directions)
	{
		var potentialObstacles = _table.Where(d => Random.value < d.Distribution.Evaluate(room.Distance)).ToArray();
		if (potentialObstacles.Length <= 0)
			return;
		var data = potentialObstacles[Random.Range(0, potentialObstacles.Length)];
		base._tile = data.ObstacleTile;
		base._data = data;
		base.PlaceObstacle(room, directions);
	}
}
