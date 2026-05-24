using UnityEngine.Tilemaps;

public class MultiTilePainter : TilePainter
{
	[SerializeField] private TileBase[] _tiles;

	public override void PaintTile(Vector2Int position)
	{
		PaintTile(position, _tilemap, _tiles[Random.Range(0, _tiles.Length)]);
	}

	public override void PaintTiles(IEnumerable<Vector2Int> positions)
	{
		PaintTiles(positions, _tilemap, _tiles);
	}
}
