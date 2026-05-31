public readonly record struct RoomInfo(Vector2Int Origin, HashSet<Vector2Int> Tiles, float Distance);

public interface IRoomPostProcessor : IComparable<IRoomPostProcessor>
{
	bool enabled { get; }
	int Order { get => 0; }

	void Clear() { }
	void ProcessRoom(RoomInfo room);

	int IComparable<IRoomPostProcessor>.CompareTo(IRoomPostProcessor other) => this.Order.CompareTo(other.Order);
}
