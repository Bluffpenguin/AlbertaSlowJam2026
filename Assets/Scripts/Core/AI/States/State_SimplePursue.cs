using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimplePursue : AIState
{
	float pathUpdateDelay = 0.5f;
	float pursueSpeed = 8f;
	public State_SimplePursue(EnemyInfo _enemyInfo, Transform _player)
        : base(_enemyInfo, _player)
    {
        stateName = STATE.PURSUE;
    }

	public override void Enter()
	{
		UpdatePathToPlayer();
		base.Enter();
	}

	public override void Update()
	{
		
		base.Update();
	}

	public override void FixedUpdate()
	{
		Vector3Int playerPos = enemyInfo.rm.navTileMap.WorldToCell(player.position);
		float distance = Vector3Int.Distance(playerPos, enemyInfo.rm.navTileMap.WorldToCell(enemyInfo.npc.transform.position));
		if (enemyInfo.rm.navTileMap.GetTile(playerPos) == null)
		{
			// Player left room
			if (enemyInfo.defaultState == STATE.PATROL)
				nextState = new State_SimplePatrol(enemyInfo, player);
			else
				nextState = new State_SimpleIdle(enemyInfo, player);
			
			stage = EVENT.EXIT;
			return;
		}
		
		if (distance <= enemyInfo.attackRange && CanSeePlayer())
		{
			// Enter attack state
			nextState = new State_SimpleAttack(enemyInfo, player);
			stage = EVENT.EXIT;
			return;
		}

		if (currentWP == path.Count)
			return;


		if (Vector2.Distance(path[currentWP].getId().transform.position, enemyInfo.npc.transform.position) < accuracy)
		{
			currentNode = path[currentWP].getId();
			currentWP++;
		}

		if (currentWP < path.Count)
		{
			Transform goal = path[currentWP].getId().transform;
			Vector2 lookAtGoal = new Vector2(goal.position.x, goal.position.y);

			Vector2 direction = (lookAtGoal - (Vector2)enemyInfo.npc.transform.position).normalized;

			LookTowards(goal.position);
			enemyInfo.rb.AddForce(pursueSpeed * Time.fixedDeltaTime * direction, ForceMode2D.Impulse);
			//enemyInfo.npc.transform.Translate(patrolSpeed * Time.deltaTime * direction);
		}


		base.FixedUpdate();
	}

	void UpdatePathToPlayer()
	{
		if (path.Count == 0 || currentWP > 0)
		{
			Node playerNode = enemyInfo.rm.GetNode(PlayerPosition);
			Node enemyNode = enemyInfo.rm.GetNode(EnemyPosition);
			currentWP = 0;
			if (enemyInfo.rm.pf.AStar(enemyNode, playerNode))
			{
				path = enemyInfo.rm.pf.ShavePath(enemyInfo.rm.pf.pathList);
			}
			else
			{
				// Player left room
				if (enemyInfo.defaultState == STATE.PATROL)
					nextState = new State_SimplePatrol(enemyInfo, player);
				else
					nextState = new State_SimpleIdle(enemyInfo, player);
				return;
			}
		}
		

		enemyInfo.rm.StartCoroutine(WaitToUpdatePath());
	}

	public override void Exit()
	{
		base.Exit();
	}

	IEnumerator WaitToUpdatePath()
	{
		yield return new WaitForSeconds(pathUpdateDelay);
		UpdatePathToPlayer();
	}
}
