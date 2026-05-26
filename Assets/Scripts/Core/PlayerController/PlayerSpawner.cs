public class PlayerSpawner : SingleTilePainter, IRoomPostProcessor
{
	[SerializeField] private int _processingOrder;

	int IRoomPostProcessor.Order => _processingOrder;

	public void ProcessRoom(RoomInfo room)
	{
		if (room.Distance == 0) {
			PaintTile(room.Origin);
			room.Tiles.Remove(room.Origin);
		}
	}
}
