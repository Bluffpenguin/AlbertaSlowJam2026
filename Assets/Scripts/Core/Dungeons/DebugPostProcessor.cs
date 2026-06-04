public class DebugPostProcessor : SingleTilePainter, IRoomPostProcessor
{
	[SerializeField] private int _processingOrder = 0;

	public int Order => _processingOrder;

	public void ProcessRoom(RoomInfo room)
	{
		base.PaintTiles(room.Tiles);
	}
}
