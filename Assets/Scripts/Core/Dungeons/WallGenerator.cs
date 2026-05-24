using UnityEngine;

public static class WallGenerator
{
	public static HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floor, TilePainter painter)
	{
		HashSet<Vector2Int> walls = FindWallsInDirections(floor, Direction2D.all);
		foreach (var position in walls) {
			painter.PaintTile(position);
		}
		return walls;
	}

	private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floor, IEnumerable<Vector2Int> directions)
	{
		var walls = new HashSet<Vector2Int>();
		foreach (var position in floor) {
			foreach (var direction in directions) {
				var neighbour = position + direction;
				if (!floor.Contains(neighbour)) {
					walls.Add(neighbour);
				}
			}
		}
		return walls;
	}
}
