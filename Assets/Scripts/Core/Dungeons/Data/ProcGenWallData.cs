[CreateAssetMenu(fileName = "WallData_", menuName = "Proc Gen/Wall Data")]
public class ProcGenWallData : ScriptableObject
{
    [SerializeField]
    [Range(1, 5)] private int _thickness = 1;

	public int Thickness { get => _thickness; }
}
