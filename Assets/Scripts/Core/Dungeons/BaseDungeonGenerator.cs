public abstract class BaseDungeonGenerator : MonoBehaviour
{
	[SerializeField] protected Vector2Int _startPosition = Vector2Int.zero;

	public abstract void Clear();
	public abstract void Generate();
}
