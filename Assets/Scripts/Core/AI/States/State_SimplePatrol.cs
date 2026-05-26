using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;

public class State_SimplePatrol : AIState
{
	float patrolSpeed = 2.5f;
	public State_SimplePatrol(GameObject _npc, Animator _anim, Transform _player, RoomManager _rm, Transform _heading)
        : base(_npc, _anim, _player, _rm, _heading)
    {
        name = STATE.PATROL;
    }

	public override void Enter()
	{
        base.Enter();
	}

	public override void Update()
	{
		if (path.Count == 0 || currentWP == path.Count)
			return;

		if (Vector2.Distance(path[currentWP].getId().transform.position, npc.transform.position) < accuracy)
		{
			currentNode = path[currentWP].getId();
			currentWP++;
		}

		if (currentWP < path.Count)
		{
			Transform goal = path[currentWP].getId().transform;
			Vector2 lookAtGoal = new Vector2(goal.position.x, goal.position.y);

			Vector2 direction = lookAtGoal - (Vector2)npc.transform.position;


			npc.transform.Translate(direction * patrolSpeed * Time.deltaTime);
		}
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}
}
