using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class NodePostProcessor : SingleTilePainter, IRoomPostProcessor
{
	[SerializeField] TileBase manager;
	[SerializeField] TileBase enemy;
	[SerializeField] Tilemap itemMap;
	public void ProcessRoom(RoomInfo room)
	{
		if (room.Distance == 0) return;
		base.PaintTiles(room.Tiles);

		PaintTile(room.Origin, _tilemap, manager);
		GameObject rmObj = _tilemap.GetInstantiatedObject((Vector3Int)room.Origin);
		RoomManager rm = rmObj.GetComponent<RoomManager>();
		rm.itemTileMap = itemMap;
		rm.GenerateLink(room.Tiles, _tilemap, false);

		// Spawn Enemies
		Debug.Log(room.Distance);
		float enemyWeight = 1 + room.Distance * 1.5f;
		float leftOver = enemyWeight - Mathf.Floor(enemyWeight);
		int numOfEnemies = 0;
		if (leftOver < 0.3f)
		{
			numOfEnemies = Mathf.FloorToInt(enemyWeight);
		}
		else
		{
			numOfEnemies = Mathf.FloorToInt(enemyWeight);
			// Roll to see if an additional enemy is added
			float rand = Random.Range(0f, 1f);
			if (rand <= leftOver)
			{
				numOfEnemies++;
			}
		}
		rm.expectedEnemiesCount = numOfEnemies;
		int numOfPatrollingEnemies = 0;
		for (int i = 0; i < numOfEnemies; i++)
		{
			
			if (i >= 1)
			{
				// Determine if enemy will patrol or guard
				int rand = Random.Range(0, 2);
				if (rand == 0)
				{
					
					numOfPatrollingEnemies++;
				}
				
			}
			else
			{
				numOfPatrollingEnemies++;
			}
		}

		List<Vector2Int>[] patrols = GeneratePatrols(room, 3, numOfPatrollingEnemies);

		StartCoroutine(WaitForColliders(rm, room, patrols, numOfPatrollingEnemies, numOfEnemies - numOfPatrollingEnemies));
		
	}


	/// <summary>
	/// Rules:
	///		- 1: A stop in the patrol must be atleast 5 tiles away from all other stops in all patrols
	/// </summary>
	/// <param name="room">Various info about the room</param>
	/// <param name="patrolSize">The numbers of stops in the patrol</param>
	/// <returns></returns>
	List<Vector2Int>[] GeneratePatrols(RoomInfo room, int patrolSize, int numOfPatrols)
	{
		List<Vector2Int> arrayFromHashset = room.Tiles.ToList();
		List<Vector2Int> usedPositions = new List<Vector2Int>();
		List<Vector2Int>[] allPatrols = new List<Vector2Int>[numOfPatrols];
		HashSet<Vector2Int> usableTiles = room.Tiles;
		usableTiles.Remove(room.Origin);


		for (int i = 0; i <numOfPatrols; i++)
		{
			allPatrols[i] = new List<Vector2Int>();
		}

		int currPatrol = 0;
		int currStop = 0;
		foreach (Vector2Int tile in usableTiles)
		{
			if (currStop == 0 && tile != room.Origin)
			{
				allPatrols[currPatrol].Add(tile);
				usedPositions.Add(tile);
				currStop++;
			}
			else if (CheckRule1(tile, usedPositions))
			{
				allPatrols[currPatrol].Add(tile);
				usedPositions.Add(tile);
				currStop++;
			}

			if (currStop == patrolSize)
			{
				currPatrol++;
				currStop = 0;
			}

			if (currPatrol == numOfPatrols)
			{
				break;
			}
		}

		if (currPatrol != numOfPatrols)
		{
			if (currStop <= 1)
			{
				
				for (int i = currPatrol; i < numOfPatrols; i++)
				{
					allPatrols[i] = null;
				}
			}
			else 
			{
				for (int i = currPatrol + 1; i < numOfPatrols; ++i)
				{
					allPatrols[i] = null;
				}
			}
		}
		

		return allPatrols;
	}

	// Check if the provided tile is at least 5 tiles away from any other stop
	bool CheckRule1(Vector2Int pos, List<Vector2Int> previousStops)
	{
		foreach (Vector2Int stop in previousStops)
		{
			if (Vector2Int.Distance(pos, stop) < 5)
			{
				return false;
			}
		}
		return true;
	}

	List<Node> GetCollectivePatrolPath(List<Vector2Int> patrol, RoomManager rm)
	{
		List<Node> col_patrolPath = new List<Node>();
		for (int i = 0; i + 1 < patrol.Count; i++)
		{
			rm.pf.AStar(rm.GetNode(patrol[i]), rm.GetNode(patrol[i + 1]));
			col_patrolPath.AddRange(rm.pf.ShavePath(rm.pf.pathList));
		}

		// Remove duplicates
		col_patrolPath = col_patrolPath.Distinct().ToList();
		return col_patrolPath;
	}

	bool IsPointOnPath(List<Node> nodes, Vector2Int pos)
	{
		for (int i = 0; i + 1 < nodes.Count; i++)
		{
			if (IsPointOnLine(nodes[i].position, nodes[i+1].position, pos))
			{
				return true;
			}
		}
		return false;
	}

	

	bool IsPointOnLine(Vector2Int start, Vector2Int end, Vector2Int point, float range = 0.01f)
	{
		float totalLength = Vector2Int.Distance(start, end);
		float d1 = Vector2Int.Distance(point, start);
		float d2 = Vector2Int.Distance(point, end);

		
		return Mathf.Abs(totalLength - (d1 + d2)) < range;
	}

	Node GetGuardPosition(List<Node>[] shavedPatrols, List<Node> occupiedGuardSpots, RoomInfo room, RoomManager rm)
	{
		bool invalidTile = false;
		foreach (Vector2Int tile in room.Tiles)
		{
			invalidTile = false;
			if (tile != room.Origin)
			{
				// Check if the node is already in use
			foreach (Node node in occupiedGuardSpots)
			{
				if (node.position == tile)
				{
					invalidTile = true;
					break;
				}
			}

			// If the node is still valid, check if it intersects with any patrols
			if (!invalidTile)
			{
				foreach (List<Node> list in shavedPatrols)
				{
					if (IsPointOnPath(list, tile))
					{
						invalidTile = true;
						break;
					}
				}
			}

			// If the node is still valid, return it
			if (!invalidTile)
			{
				return rm.GetNode(tile);
			}
			}
			
		}

		// No valid Node
		return null;
	}

	IEnumerator WaitForColliders(RoomManager rm, RoomInfo room, List<Vector2Int>[] allPatrols, int numOfPatrollingEnemies, int numOfIdleEnemies)
	{
		yield return new WaitForSeconds(0.5f);
		int currPatrol = 0;
		int enemiesSpawned = 0;
		int idleEnemiesToBeSpawned = numOfIdleEnemies;
		List<Node>[] shavedPatrols = new List<Node>[allPatrols.Length];
		for (int i = 0; i < numOfPatrollingEnemies; i++)
		{
			if (allPatrols[currPatrol] != null)
			{
				List<Node> patrol = GetCollectivePatrolPath(allPatrols[currPatrol], rm);
				shavedPatrols[currPatrol] = patrol;

				_tilemap.SetTile((Vector3Int)patrol[0].position, enemy);
				GameObject obj = _tilemap.GetInstantiatedObject((Vector3Int)patrol[0].position);
				patrol[0].setId(obj);

				SimpleEnemy enemyScript = obj.GetComponentInChildren<SimpleEnemy>();
				enemyScript.enemyInfo.enemyId = enemiesSpawned;
				enemiesSpawned++;

				enemyScript.enemyInfo.defaultState = AIState.STATE.PATROL; 
				enemyScript.enemyInfo.rm = rm;
				
				rm.AddPatrol(patrol, enemyScript.enemyInfo.enemyId);
				enemyScript.gameObject.transform.position = patrol[0].getId().transform.position;
			}
			else
			{
				idleEnemiesToBeSpawned++;
			}
			currPatrol++;
		}
		

		// After all patrols are set, set up guard positions
		List<Node> previousGuardSpots = new List<Node>();
		for (int i = 0; i < idleEnemiesToBeSpawned; i++)
		{
			Node guardSpot = GetGuardPosition(shavedPatrols, previousGuardSpots, room, rm);
			if (guardSpot != null)
			{
				_tilemap.SetTile((Vector3Int)guardSpot.position, enemy);
				GameObject obj = _tilemap.GetInstantiatedObject((Vector3Int)guardSpot.position);
				guardSpot.setId(obj);


				SimpleEnemy enemyScript = obj.GetComponentInChildren<SimpleEnemy>();
				enemyScript.enemyInfo.enemyId = enemiesSpawned;
				enemiesSpawned++;

				enemyScript.enemyInfo.defaultState = AIState.STATE.IDLE;
				enemyScript.enemyInfo.rm = rm;


				rm.AddGuard(guardSpot, enemyScript.enemyInfo.enemyId);
				previousGuardSpots.Add(guardSpot);
				enemyScript.gameObject.transform.position = guardSpot.getId().transform.position;
			}
		}
		
	}

	
}
