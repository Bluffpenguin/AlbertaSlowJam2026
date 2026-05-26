[CreateAssetMenu(fileName = "Preset", menuName = "Proc Gen/Room Data")]
public class ProcGenRoomData : ScriptableObject
{
	[SerializeField] protected int _iterations = 10;
	[SerializeField] protected int _length = 10;
	[Tooltip("Rooms will be larger and more cave-like.")]
	[SerializeField] protected bool _largeRooms = true;
	[Min(1)]
	[SerializeField] protected Vector2Int _maxRoomSize = Vector2Int.one;

	// modifying scriptable object values at runtime will alter them permanently, so values should not be settable.
	public int Iterations => _iterations;
	public int Length => _length;
	public bool LargeRooms => _largeRooms;
	public Vector2Int MaxRoomSize => _maxRoomSize;
}
