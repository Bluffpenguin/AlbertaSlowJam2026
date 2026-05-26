using UnityEngine.Pool;
using System.Linq;

public static class WallGenerator
{
	public static void CreateWalls(HashSet<Vector2Int> floor, TilePainter painter, int thickness = 2)
	{
		CreateWalls(floor, painter, thickness, out _).Dispose();
	}

	public static IDisposable CreateWalls(HashSet<Vector2Int> floor, TilePainter painter, out HashSet<Vector2Int> walls)
		=> CreateWalls(floor, painter, 2, out walls);
	public static IDisposable CreateWalls(HashSet<Vector2Int> floor, TilePainter painter, int thickness, out HashSet<Vector2Int> walls)
	{
		var obj = HashSetPool<Vector2Int>.Get(out walls);
		using var _ = HashSetPool<Vector2Int>.Get(out var tiles);
		tiles.UnionWith(floor);

		Vector2Int[] directions = Direction2D.GetDirections(Direction2D.Type.Cardinal | Direction2D.Type.Ordinal);

		for (int i = 0; i < thickness; i++) {
			var layer = FindWallsInDirections(tiles, directions).ToArray();
			walls.UnionWith(layer);
			tiles.UnionWith(layer);
		}

		painter.PaintTiles(walls);

		return obj;
	}

	private static IEnumerable<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floor, IEnumerable<Vector2Int> directions)
	{
		return from position in floor
			   from direction in directions
			   let neighbour = position + direction
			   where !floor.Contains(neighbour)
			   select neighbour;
	}
}
