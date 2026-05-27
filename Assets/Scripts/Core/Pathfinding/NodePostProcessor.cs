using System.Linq;
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
		agent.enemyInfo.rm = rm;
		agent.enemyInfo.enemyId = 0;
		agent.enemyInfo.defaultState = AIState.STATE.PATROL;
		rm.AddPatrol(GenerateRandomPatrol(room));
		
	}

	List<Vector2Int> GenerateRandomPatrol(RoomInfo room)
	{
		int patrolSize = 3;
		List<Vector2Int> arrayFromHashset = room.Tiles.ToList();
		List<Vector2Int> patrol = new List<Vector2Int>();
		while (patrol.Count < patrolSize)
		{
			int index = Random.Range(0, arrayFromHashset.Count);
			if (!patrol.Contains(arrayFromHashset[index]))
			{
				patrol.Add(arrayFromHashset[index]);
			}
		}
		return patrol;
	}
}
