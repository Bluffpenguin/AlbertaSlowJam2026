using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimpleIdle : AIState
{
	Node guardNode;
	bool atGuardNode = false;
	float returnSpeed = 3f;
	public State_SimpleIdle(EnemyInfo _enemyInfo, Transform _player)
        : base(_enemyInfo, _player)
    {
        stateName = STATE.IDLE;
    }

	public override void Enter()
	{
		guardNode = enemyInfo.rm.GetGuardSpot(enemyInfo.enemyId);

		if (Vector3.Distance(enemyInfo.npc.transform.position, guardNode.getId().transform.position) < accuracy)
		{
			atGuardNode = true;
		}
		else
		{
			Vector2Int currPos = (Vector2Int)enemyInfo.rm.navTileMap.WorldToCell(enemyInfo.npc.transform.position);
			Move_Shaved(currPos, guardNode.position);
		}
		base.Enter();
	}

	public override void Update()
	{
		if (CanSeePlayer())
		{
			nextState = new State_SimplePursue(enemyInfo, player);
			stage = EVENT.EXIT;
		}
		base.Update();
	}

	public override void FixedUpdate()
	{
		if (atGuardNode) return;

		if (currentWP == path.Count)
		{
			atGuardNode = true;
			enemyInfo.npc.transform.position = guardNode.getId().transform.position;
			return;
		}

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
			enemyInfo.rb.AddForce(returnSpeed * Time.fixedDeltaTime * direction, ForceMode2D.Impulse);
		}

		base.FixedUpdate();
	}

	public override void Exit()
	{
		base.Exit();
	}
}
