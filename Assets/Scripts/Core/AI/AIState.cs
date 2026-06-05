using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AIState 
{
	public enum STATE
	{
		IDLE, PATROL, PURSUE, ATTACK
	};

	public enum EVENT
	{
		ENTER, UPDATE, EXIT
	};

	public STATE stateName;
	protected EVENT stage;
	protected EnemyInfo enemyInfo;
	protected Transform player;
	protected AIState nextState;

	internal float visDist = 3.0f;
	internal float visAngle = 30.0f;

	protected GameObject startObj;
	protected GameObject endObj;
	protected List<Node> path = new List<Node>();
	protected float accuracy = 0.1f;
	protected int currentWP = 0;
	protected GameObject currentNode = null;

	protected Vector2Int PlayerPosition
	{
		get
		{
			return (Vector2Int)enemyInfo.rm.navTileMap.WorldToCell(player.position);
		}
	}

	protected Vector2Int EnemyPosition
	{
		get
		{
			return (Vector2Int)enemyInfo.rm.navTileMap.WorldToCell(enemyInfo.npc.transform.position);
		}
	}


	public AIState(EnemyInfo _enemyInfo, Transform _player)
	{
		
		stage = EVENT.ENTER;
		player = _player;
		enemyInfo = _enemyInfo;
		
	}

	public virtual void Enter() { stage = EVENT.UPDATE; }
	public virtual void Update() { stage = EVENT.UPDATE; }
	public virtual void FixedUpdate() { stage = EVENT.UPDATE; }
	public virtual void Exit() { stage = EVENT.EXIT; }

	public AIState Process()
	{
		if (stage == EVENT.ENTER) Enter();
		if (stage == EVENT.UPDATE) Update();
		if (stage == EVENT.EXIT)
		{
			Exit();
			return nextState;
		}
		return this;
	}

	public AIState ProcessFixed()
	{
		if (stage == EVENT.UPDATE) FixedUpdate();
		return this;
	}

	public void HandleAnimation()
	{
		if (enemyInfo.rb.linearVelocity.magnitude > 0.1f)
		{
			// Enemy is moving
			enemyInfo.anim.SetBool("isMoving", true);
		}
		else
		{
			// Enemy is idle
			enemyInfo.anim.SetBool("isMoving", false);
			
		}

		// Apply directoin
		if (enemyInfo.heading.up.x > 0)
		{
			// right side
			

			if (enemyInfo.heading.up.y > 0.5f)
			{
				// up
				//enemyInfo.spriteRenderer.transform.rotation = Quaternion.Euler(enemyInfo.spriteDirections[0].rotation);
				//if (enemyInfo.spriteDirections[0].flipX) enemyInfo.spriteRenderer.flipX = true;
				//else enemyInfo.spriteRenderer.flipX = false;

			}
			else if (enemyInfo.heading.up.y < -0.5f)
			{
				// down
				//enemyInfo.spriteRenderer.transform.rotation = Quaternion.Euler(enemyInfo.spriteDirections[2].rotation);
				//if (enemyInfo.spriteDirections[2].flipX) enemyInfo.spriteRenderer.flipX = true;
				//else enemyInfo.spriteRenderer.flipX = false;
			}
			else
			{
				// middle
				//enemyInfo.spriteRenderer.transform.rotation = Quaternion.Euler(enemyInfo.spriteDirections[1].rotation);
				//if (enemyInfo.spriteDirections[1].flipX) enemyInfo.spriteRenderer.flipX = true;
				//else enemyInfo.spriteRenderer.flipX = false;
			}
		}
		else
		{
			// left side
			

			if (enemyInfo.heading.up.y > 0.5f)
			{
				// up
				//enemyInfo.spriteRenderer.transform.rotation = Quaternion.Euler(enemyInfo.spriteDirections[5].rotation);
				//if (enemyInfo.spriteDirections[5].flipX) enemyInfo.spriteRenderer.flipX = true;
				//else enemyInfo.spriteRenderer.flipX = false;

			}
			else if (enemyInfo.heading.up.y < -0.5f)
			{
				// down
				//enemyInfo.spriteRenderer.transform.rotation = Quaternion.Euler(enemyInfo.spriteDirections[3].rotation);
				//if (enemyInfo.spriteDirections[3].flipX) enemyInfo.spriteRenderer.flipX = true;
				//else enemyInfo.spriteRenderer.flipX = false;
			}
			else
			{
				// middle
				//enemyInfo.spriteRenderer.transform.rotation = Quaternion.Euler(enemyInfo.spriteDirections[4].rotation);
				//if (enemyInfo.spriteDirections[4].flipX) enemyInfo.spriteRenderer.flipX = true;
				//else enemyInfo.spriteRenderer.flipX = false;
			}
		}
	}

	public bool CanSeePlayer()
	{
		
		if (player == null) 
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
			return false;
		}
		Vector3 direction = player.position - enemyInfo.npc.transform.position;
		float angle = Vector3.Angle(direction, enemyInfo.heading.up);
		float scaledDist = direction.magnitude * enemyInfo.rm.navTileMap.cellSize.magnitude;

		if (scaledDist  < visDist && angle < visAngle)
		{
			 direction = direction.normalized;
			float distance = Vector2.Distance(enemyInfo.npc.transform.position, player.position);

			if (!Physics2D.Raycast(enemyInfo.npc.transform.position, direction, distance, enemyInfo.sightObstructions))
			{
				return true;
			}
			
		}
		return false;
	}

	public void Move(GameObject start, GameObject goal)
	{
		currentWP = 0;
		path.Clear();

		Node startNode = enemyInfo.rm.GetNode(start);
		if (startNode == null) return;

		Node endNode = enemyInfo.rm.GetNode(goal);
		if (endNode == null) return;

		if (enemyInfo.rm.pf.AStar(startNode, endNode))
		{
			path = enemyInfo.rm.pf.pathList;
			enemyInfo.npc.transform.position = start.transform.position;
		}

	}

	public void Move(Vector2Int start, Vector2Int goal)
	{
		currentWP = 0;
		path.Clear();

		Node startNode = enemyInfo.rm.GetNode(start);
		if (startNode == null) return;

		Node endNode = enemyInfo.rm.GetNode(goal);
		if (endNode == null) return;

		if (enemyInfo.rm.pf.AStar(startNode, endNode))
		{
			path = enemyInfo.rm.pf.pathList;
			
		}

	}

	public void Move_Shaved(GameObject start, GameObject goal)
	{
		currentWP = 0;
		path.Clear();

		Node startNode = enemyInfo.rm.GetNode(start);
		if (startNode == null) return;

		Node endNode = enemyInfo.rm.GetNode(goal);
		if (endNode == null) return;

		if (enemyInfo.rm.pf.AStar(startNode, endNode))
		{
			path = enemyInfo.rm.pf.ShavePath(enemyInfo.rm.pf.pathList);
		}

	}

	public void Move_Shaved(Vector2Int start, Vector2Int goal)
	{
		currentWP = 0;
		path.Clear();

		Node startNode = enemyInfo.rm.GetNode(start);
		if (startNode == null) return;

		Node endNode = enemyInfo.rm.GetNode(goal);
		if (endNode == null) return;

		if (enemyInfo.rm.pf.AStar(startNode, endNode))
		{
			
			path = enemyInfo.rm.pf.ShavePath(enemyInfo.rm.pf.pathList);
		}

	}

	public void LookTowards(Vector2 pos)
	{
		//Quaternion rot = Quaternion.LookRotation(Vector3.forward, pos);

		float angle = Mathf.Atan2(pos.y - enemyInfo.heading.position.y, pos.x - enemyInfo.heading.position.x) * Mathf.Rad2Deg;
		Quaternion rot = Quaternion.Euler(0, 0, angle - 90);
		enemyInfo.heading.rotation = rot;

		//enemyInfo.heading.rotation = rot;

		// TODO: Animation
	}


}
