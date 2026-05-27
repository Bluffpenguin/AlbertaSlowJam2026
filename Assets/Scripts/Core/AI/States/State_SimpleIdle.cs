using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class State_SimpleIdle : AIState
{ 
	public State_SimpleIdle(EnemyInfo _enemyInfo, Transform _player)
        : base(_enemyInfo, _player)
    {
        stateName = STATE.IDLE;
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
			nextState = new State_SimplePatrol(enemyInfo, player);
		}
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}
}
