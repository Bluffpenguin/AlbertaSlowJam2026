using UnityEngine;

public class NodePostProcessor : SingleTilePainter, IRoomPostProcessor
{
	[SerializeField] private PathfindingManager _manager;

	public void ProcessRoom(RoomInfo room)
	{
		base.PaintTiles(room.Tiles);
		// Note: this doesn't work b/c the dictionary is cleared when this method is called
		_manager.GenerateLink(room.Tiles, _tilemap, false);
	}
}
