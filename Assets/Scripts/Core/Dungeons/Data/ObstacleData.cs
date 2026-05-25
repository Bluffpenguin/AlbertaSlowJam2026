using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Data/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
	[Flags]
	public enum SpaceRequirements
	{
		None = 0,
		NextToWall = 1 << 0,
		OpenSpace = 1 << 1,
	}

	[SerializeField] private bool _impassable = true;
	[SerializeField] private Direction2D.Type _checkDirections = Direction2D.Type.Cardinal;
	[SerializeField] private SpaceRequirements _requirements;

	public bool Impassable => _impassable;
	public Direction2D.Type CheckDirections { get => _checkDirections; }
	public SpaceRequirements Requirements { get => _requirements; }
}
