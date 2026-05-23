using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : MonoBehaviour
{
	[SerializeField]
	private Tilemap floorMap;
	[SerializeField]
	private TileBase floorTile;

	public void PaintFloorTiles(IEnumerable<Vector2Int> positions, bool overwrite)
	{
		if (overwrite)
			floorMap.ClearAllTiles();
		PaintTiles(positions, floorMap, floorTile);
	}

	private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, params TileBase[] tiles)
	{
		if (tiles?.Length is null or 0)
			throw new ArgumentNullException(nameof(tiles), "No tiles to paint.");

		foreach (var position in positions) {
			var tile = tiles[Random.Range(0, tiles.Length)];
			PaintTile(position, tilemap, tile);
		}
	}

	private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
	{
		foreach (var position in positions) {
			PaintTile(position, tilemap, tile);
		}
	}

	private void PaintTile(Vector2Int position, Tilemap tilemap, TileBase tile)
	{
		if (tilemap == null)
			throw new ArgumentNullException(nameof(tilemap));
		if (tile == null)
			throw new ArgumentNullException(nameof(tile));

		var tilePosition = tilemap.WorldToCell((Vector3Int)position);
		tilemap.SetTile(tilePosition, tile);
	}
}
