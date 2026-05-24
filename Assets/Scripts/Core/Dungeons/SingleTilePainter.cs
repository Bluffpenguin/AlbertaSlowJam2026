using UnityEngine.Tilemaps;

public class SingleTilePainter : TilePainter
{
	[SerializeField] private TileBase _tile;

	public override void PaintTile(Vector2Int position)
	{
		PaintTile(position, _tilemap, _tile);
	}

	public override void PaintTiles(IEnumerable<Vector2Int> positions)
	{
		PaintTiles(positions,  _tilemap, _tile);
	}
}
