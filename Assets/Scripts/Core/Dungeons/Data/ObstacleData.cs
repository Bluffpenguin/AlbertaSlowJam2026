[CreateAssetMenu(fileName = "Obstacle_", menuName = "Proc Gen/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
	[Flags]
	public enum SpaceRequirements
	{
		None = 0,
		NextToWall = 1 << 0,
		OpenSpace = 1 << 1,
	}

	[Tooltip("This obstacle will block any others from being placed its tiles.")]
	[SerializeField] private bool _impassable = true;
	[SerializeField] private Direction2D.Type _checkDirections = Direction2D.Type.Cardinal;
	[SerializeField] private SpaceRequirements _requirements;
	[SerializeField, Min(1)] private Vector2Int _dimensions = Vector2Int.one;
	
	public bool Impassable => _impassable;
	public Direction2D.Type CheckDirections { get => _checkDirections; }
	public SpaceRequirements Requirements { get => _requirements; }
	public Vector2Int Dimensions { get => _dimensions; }
}
