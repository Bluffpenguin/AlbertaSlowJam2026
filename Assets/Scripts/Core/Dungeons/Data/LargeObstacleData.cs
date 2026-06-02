[Obsolete]
public class LargeObstacleData : ObstacleData
{
	[SerializeField]
	[Min(1)] private Vector2Int _size = Vector2Int.one;
	[SerializeField]
	private bool _allowMirror = true;

	public Vector2Int Size { get => _size; }
	public bool AllowMirror { get => _allowMirror; }
}
