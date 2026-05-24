using System.Linq;
using UnityEngine;

public class TreeGenerator : SimpleRandomWalkGenerator
{
	public enum StartingDirection { Random, North, East, South, West }

	[SerializeField]
	[Min(1)] protected int _maxBranches = 1;
	[SerializeField]
	[Min(0)] protected int _minStepsBeforeBranching;
	[SerializeField]
	[Min(1)]
	protected int _stepLength = 10;
	[SerializeField]
	[Min(1)] protected int _walkLength = 1;

	private Dictionary<Vector2Int, RoomInfo> rooms;

	public override void Generate()
	{
		Clear();
		rooms.Clear();
		GenerateBranches();

	}

	private HashSet<Vector2Int> GenerateBranches()
	{
		List<Branch> branches = new();
		var initialBranch = new Branch() { currentPosition = _startPosition };

		// ...
		// TODO: Finish this

		HashSet<Vector2Int> floor = new();
		for (int i = 0; i < branches.Count; i++) {
			floor.UnionWith(branches[i].floor);
		}

		return floor;
	}

	private class Branch
	{
		public readonly HashSet<Vector2Int> floor;
		public readonly Vector2Int[] bannedDirections;
		public Vector2Int currentPosition;
		private int _index;

		public Branch()
		{
			floor = new HashSet<Vector2Int>();
			bannedDirections = new Vector2Int[2];
		}

		public Vector2Int Advance(int steps, int iterations)
		{
			for (int i = 0; i < iterations; i++) {
				var direction = GetRandomDirection();
				for (int j = 0; j < steps; j++) {
					currentPosition += direction;
					if (!floor.Contains(currentPosition))
						floor.Add(currentPosition);
				}
			}
			return currentPosition;
		}

		private Vector2Int GetRandomDirection()
		{
			Vector2Int direction;
			using (UnityEngine.Pool.HashSetPool<Vector2Int>.Get(out var possibleDirections)) {
				possibleDirections.UnionWith(Direction2D.cardinals);
				possibleDirections.ExceptWith(bannedDirections);
				direction = possibleDirections.ElementAt(Random.Range(0, possibleDirections.Count));
				bannedDirections[_index] = -direction;
				_index = (_index + 1) % bannedDirections.Length;
			}
			return direction;
		}
	}
}
