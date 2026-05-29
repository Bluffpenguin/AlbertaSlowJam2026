using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Obstacle_", menuName = "Proc Gen/Multi Obstacle Data")]
public class MultiObstacleData : ObstacleData
{
	[SerializeField] private TileBase _obstacleTile;
	[SerializeField] private AnimationCurve _distribution = new(new Keyframe(0, 1), new Keyframe(1, 1));

	public AnimationCurve Distribution { get => _distribution; }
	public TileBase ObstacleTile { get => _obstacleTile; }
}
