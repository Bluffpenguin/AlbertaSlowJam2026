using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimpleAttack : AIState
{
	bool tempWait = false;
	public State_SimpleAttack(EnemyInfo _enemyInfo, Transform _player)
        : base(_enemyInfo, _player)
    {
        stateName = STATE.ATTACK;
    }

	public override void Enter()
	{
		Node fleeDestination = enemyInfo.rm.GetRandomFleePosition(player, 5);
		Node currentNode = enemyInfo.rm.GetNode((Vector2Int)enemyInfo.rm.tileMap.WorldToCell(enemyInfo.npc.transform.position));
		
		// Steal Item
		

		// Get path for running away
		Move_Shaved(currentNode.position, fleeDestination.position);
		
		base.Enter();
	}

	public override void Update()
	{
		

		
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}


}
