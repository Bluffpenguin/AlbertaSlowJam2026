using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimpleAttack : AIState
{
	ItemStack stolenItem;
	float fleeSpeed = 5f;

	float stallDuration = 2f;
	bool stalling = false;

	public State_SimpleAttack(EnemyInfo _enemyInfo, Transform _player)
        : base(_enemyInfo, _player)
    {
        stateName = STATE.ATTACK;
    }

	public override void Enter()
	{
		Node fleeDestination = enemyInfo.rm.GetRandomFleePosition(player, 5);
		Node currentNode = enemyInfo.rm.GetNode((Vector2Int)enemyInfo.rm.navTileMap.WorldToCell(enemyInfo.npc.transform.position));

		// TODO: Stun Player

		// Steal Item
		int slot = Random.Range(0, Player.Inventory.Capacity);
		Player.Inventory.RemoveAt(slot, 1, out stolenItem);
		

		// Get path for running away
		Move_Shaved(currentNode.position, fleeDestination.position);
		
		base.Enter();
	}

	public override void Update()
	{
		

		
		base.Update();
	}

	public override void FixedUpdate()
	{
		if (stalling) return;


		// Upon reaching the flee destination, wait a couple seconds then return to normal behaviour
		if (currentWP == path.Count)
		{
			enemyInfo.rm.PlaceItem(path[currentWP-1].position, stolenItem);
			enemyInfo.rm.StartCoroutine(Stall());
			stalling = true;
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
			enemyInfo.rb.AddForce(fleeSpeed * Time.fixedDeltaTime * direction, ForceMode2D.Impulse);
			//enemyInfo.npc.transform.Translate(patrolSpeed * Time.deltaTime * direction);
		}

		base.FixedUpdate();
	}

	public override void Exit()
	{
		base.Exit();
	}

	IEnumerator Stall()
	{
		yield return new WaitForSeconds(stallDuration);

		// revert to default state
		if (enemyInfo.defaultState == STATE.PATROL)
			nextState = new State_SimplePatrol(enemyInfo, player);
		else
			nextState = new State_SimpleIdle(enemyInfo, player);

		stage = EVENT.EXIT;
	}
}
