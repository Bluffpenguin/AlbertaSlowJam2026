using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimplePatrol : AIState
{ 
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
		
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}
}
