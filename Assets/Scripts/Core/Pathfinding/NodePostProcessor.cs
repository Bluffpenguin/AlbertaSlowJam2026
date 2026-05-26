using UnityEngine;
using UnityEngine.Tilemaps;

public class NodePostProcessor : SingleTilePainter, IRoomPostProcessor
{
	[SerializeField] TileBase manager;
	[SerializeField] GameObject enemyPrefab;
	public void ProcessRoom(RoomInfo room)
	{
		base.PaintTiles(room.Tiles);

		PaintTile(room.Origin, _tilemap, manager);
		GameObject rmObj = _tilemap.GetInstantiatedObject((Vector3Int)room.Origin);
		RoomManager rm = rmObj.GetComponent<RoomManager>();
		rm.GenerateLink(room.Tiles, _tilemap, false);

		// Generate Enemy
		Vector3 enemyPos = _tilemap.CellToWorld((Vector3Int)room.Origin);
		GameObject enemyObj = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
		SimpleEnemy agent = enemyObj.GetComponent<SimpleEnemy>();
		agent.rm = rm;
		
	}
}
