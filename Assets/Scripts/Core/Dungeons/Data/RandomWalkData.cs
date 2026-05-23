[CreateAssetMenu(fileName = "Preset", menuName = "Data/Random Walk Data")]
public class RandomWalkData : ScriptableObject
{
	[SerializeField] protected int _iterations = 10;
	[SerializeField] protected int _length = 10;
	[Tooltip("Rooms will be larger and more cave-like.")]
	[SerializeField] protected bool _largeRooms = true;

	// modifying scriptable object values at runtime will alter them permanently, so values should not be settable.
	public int Iterations => _iterations;
	public int Length => _length;
	public bool LargeRooms => _largeRooms;
}
