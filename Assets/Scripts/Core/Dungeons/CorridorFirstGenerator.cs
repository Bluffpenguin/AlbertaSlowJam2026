using System.Linq;
using UnityEngine.Pool;
using static PlasticGui.WorkspaceWindow.Merge.MergeInProgress;

public class CorridorFirstGenerator : SimpleRandomWalkGenerator
{
	[SerializeField]
	private int _corridorLength = 14, _corridorCount = 5;
	[SerializeField]
	[Range(0, 1)]
	private float _roomPercentage = 0.8f;

	private readonly Dictionary<Vector2Int, RoomInfo> _rooms = new();

	public override void Clear()
	{
		base.Clear();
		_rooms.Clear();
	}

	public override void Generate()
	{
		Clear();
		var floor = CreateCorridors(out var potentialRooms);
		var rooms = GenerateRooms(floor, potentialRooms);
		floor.UnionWith(rooms);
		_floorPainter.PaintTiles(floor);
		WallGenerator.CreateWalls(floor, _wallPainter, _wallThickness);
		base.ApplyPostProcessing(_rooms.Values);
	}

	private HashSet<Vector2Int> GenerateRooms(HashSet<Vector2Int> floor, HashSet<Vector2Int> potentialRooms)
	{
		var rooms = new HashSet<Vector2Int>();
		int roomCount = Mathf.RoundToInt(potentialRooms.Count * _roomPercentage);

		var roomPositions = potentialRooms.OrderBy(x => Guid.NewGuid())
			// Sorts by an arbitrary value to randomize the order
			.Take(roomCount)
			.ToList();

		if (!roomPositions.Contains(_startPosition))
			roomPositions.Add(_startPosition);

		var roomsDict = DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Get();
		foreach (var position in roomPositions) {
			var room = GenerateRoom(position, _preset);
			roomsDict.Add(position, room);
			rooms.UnionWith(room);
		}

		if (_roomPercentage < 1) {
			var deadEnds = from position in floor
						   where !rooms.Contains(position)
						   let neighbours = Direction2D.cardinals.Count(dir => floor.Contains(position + dir))
						   where neighbours == 1
						   select position;

			foreach (var position in deadEnds) {
				var room = GenerateRoom(position, _preset);
				roomsDict.Add(position, room);
				rooms.UnionWith(room);
			}
		}

		var distances = roomsDict.Keys.ToDictionary(static p => p, p => Vector2Int.Distance(p, _startPosition));
		float min = distances.Values.Min();
		float max = distances.Values.Max();

		foreach (var key in roomsDict.Keys) {
			var info = new RoomInfo(key, roomsDict[key], Mathf.InverseLerp(min, max, distances[key]));
			_rooms.Add(key, info);
		}
		DictionaryPool<Vector2Int, HashSet<Vector2Int>>.Release(roomsDict);

		return rooms;
	}

	private HashSet<Vector2Int> CreateCorridors(out HashSet<Vector2Int> potentialRooms)
	{
		var floor = new HashSet<Vector2Int>();
		potentialRooms = new HashSet<Vector2Int>() { _startPosition };
		var currentPosition = _startPosition;
		for (int i = 0; i < _corridorCount; i++) {
			var corridor = ProcGen.RandomWalk(currentPosition, walkLength: 1, _corridorLength).ToArray();
			currentPosition = corridor[^1];
			potentialRooms.Add(currentPosition);
			floor.UnionWith(corridor);
		}
		return floor;
	}

	private static void NormalizeValues(float[] values)
	{
		var min = Mathf.Min(values);
		var max = Mathf.Max(values);

		for (int i = 0; i < values.Length; i++) {
			if (min != max) {
				values[i] = (values[i] - min) / (max - min);
			} else {
				values[i] = 0;
			}
		}
	}
}
