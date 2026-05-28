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
		enemyInfo.rm.StartCoroutine(TempWait());
		base.Enter();
	}

	public override void Update()
	{
		if (tempWait)
			return;


		if (CanSeePlayer())
		{
			nextState = new State_SimplePursue(enemyInfo, player);
			stage = EVENT.EXIT;
		}
		else
		{
			if (enemyInfo.defaultState == STATE.IDLE)
			{
				nextState = new State_SimpleIdle(enemyInfo, player);
				stage = EVENT.EXIT;
			}
			else
			{
				nextState = new State_SimplePatrol(enemyInfo, player);
				stage = EVENT.EXIT;
			}
		}
		
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}

	IEnumerator TempWait()
	{
		tempWait = true;
		yield return new WaitForSeconds(2);
		tempWait = false;
	}
}
