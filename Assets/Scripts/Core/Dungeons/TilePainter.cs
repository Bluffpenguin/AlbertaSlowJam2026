using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TilePainter : MonoBehaviour
{
	[SerializeField] protected Tilemap _tilemap;

	public Tilemap Tilemap => _tilemap;

	public void Clear()
	{
		_tilemap.ClearAllTiles();
	}

	public abstract void PaintTiles(IEnumerable<Vector2Int> positions);
	public abstract void PaintTile(Vector2Int position);

	public static void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, params TileBase[] tileset)
	{
		if (tileset?.Length is null or 0)
			throw new ArgumentNullException(nameof(tileset), "No tiles to paint.");

		foreach (var position in positions) {
			var tile = tileset[Random.Range(0, tileset.Length)];
			PaintTile(position, tilemap, tile);
		}
	}

	public static void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
	{
		foreach (var position in positions) {
			PaintTile(position, tilemap, tile);
		}
	}

	public static void PaintTile(Vector2Int position, Tilemap tilemap, TileBase tile)
	{
		if (tilemap == null)
			throw new ArgumentNullException(nameof(tilemap));
		if (tile == null)
			throw new ArgumentNullException(nameof(tile));

		var tilePosition = (Vector3Int)position;
		tilemap.SetTile(tilePosition, tile);
	}
}
