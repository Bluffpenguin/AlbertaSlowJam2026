public readonly record struct RoomInfo(Vector2Int Origin, HashSet<Vector2Int> Tiles, float Distance);

public interface IRoomPostProcessor
{
	void ProcessRoom(RoomInfo room);

	void Clear() { }
}
