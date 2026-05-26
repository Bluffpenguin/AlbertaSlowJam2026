[CreateAssetMenu(fileName = "CorridorData_", menuName = "Proc Gen/Corridor Data")]
public class ProcGenCorridorData : ScriptableObject
{
	[SerializeField, Min(1)] private int _corridorLength = 20;
	[SerializeField, Range(1, 3)] private int _corridorWidth = 1;
	[SerializeField, Min(0)] private int _corridorCount = 5;
	[SerializeField] private bool _roundedCorners = false;

	public int Length => _corridorLength;
	public int Width => _corridorWidth;
	public int Count => _corridorCount;
	public bool RoundedCorners => _roundedCorners;
}
