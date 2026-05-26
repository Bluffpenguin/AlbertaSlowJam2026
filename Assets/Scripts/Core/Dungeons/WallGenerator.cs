using UnityEngine.Pool;
using System.Linq;

public static class WallGenerator
{
	public static void CreateWalls(HashSet<Vector2Int> floor, TilePainter painter, ProcGenWallData data)
	{
		CreateWalls(floor, painter, data, out _).Dispose();
	}

	public static IDisposable CreateWalls(HashSet<Vector2Int> floor, TilePainter painter, ProcGenWallData data, out HashSet<Vector2Int> walls)
	{
		var obj = HashSetPool<Vector2Int>.Get(out walls);
		using var _ = HashSetPool<Vector2Int>.Get(out var tiles);
		tiles.UnionWith(floor);

		Vector2Int[] directions = Direction2D.GetDirections(Direction2D.Type.Cardinal | Direction2D.Type.Ordinal);

		for (int i = 0; i < data.Thickness; i++) {
			var layer = FindWallsInDirections(tiles, directions).ToArray();
			walls.UnionWith(layer);
			tiles.UnionWith(layer);
		}

		painter.PaintTiles(walls);

		return obj;
	}

	public static void ExtendFloorInDirections(HashSet<Vector2Int> floor, int iterations, params Vector2Int[] directions)
	{
		if (floor is null)
			throw new ArgumentNullException(nameof(floor));

		if (directions?.Length is null or 0)
			return;

		for (int i = 0; i < iterations; i++) {
			var layer = FindWallsInDirections(floor, directions).ToArray();
			floor.UnionWith(layer);
		}
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
