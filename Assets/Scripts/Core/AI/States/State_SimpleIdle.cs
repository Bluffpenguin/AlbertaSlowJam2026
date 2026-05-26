using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimpleIdle : AIState
{ 
	public State_SimpleIdle(GameObject _npc, Animator _anim, Transform _player, RoomManager _rm, Transform _heading)
        : base(_npc, _anim, _player, _rm, _heading)
    {
        name = STATE.IDLE;
    }

	public override void Enter()
	{
		
		base.Enter();
	}

	public override void Update()
	{
		if (CanSeePlayer())
		{
			Debug.Log("Found Player");
			nextState = new State_SimplePatrol(npc, anim, player, rm, heading);
		}
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}
}
