using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_SimplePatrol : AIState
{
	float patrolSpeed = 0.5f;
	int currentPatrolIndex = 0;
	private readonly float stallTimeMax = 2f;
	private readonly float stallTimeMin = 0.5f;
	private bool reversePatrol = false, stalling = false;

	public State_SimplePatrol(EnemyInfo _enemyInfo, Transform _player)
        : base(_enemyInfo, _player)
    {
        stateName = STATE.PATROL;
    }

	public override void Enter()
	{
		stalling = true;
		enemyInfo.rm.StartCoroutine(WaitForPathToBeCalculated());
		
        base.Enter();
	}

	public override void Update()
	{

		if (CanSeePlayer())
		{
			//nextState = new State_SimplePursue(enemyInfo, player);
			return;
		}

		if (stalling)
			return;

		if (currentWP == path.Count)
		{
			reversePatrol = true;
			currentWP = path.Count - 1;
			return;
			
		}
		else if (currentWP == -1)
		{
			reversePatrol = false;
			currentWP = 0;
			return;
		}



		if (Vector2.Distance(path[currentWP].getId().transform.position, enemyInfo.npc.transform.position) < accuracy)
		{
			currentNode = path[currentWP].getId();

			if (!reversePatrol)
			{
				currentWP++;
			}
			else 
				currentWP--;

		}

		if (currentWP < path.Count && currentWP != -1)
		{
			Transform goal = path[currentWP].getId().transform;
			Vector2 lookAtGoal = new Vector2(goal.position.x, goal.position.y);

			Vector2 direction = (lookAtGoal - (Vector2)enemyInfo.npc.transform.position).normalized;

			LookTowards(goal.position);
			enemyInfo.npc.transform.Translate(patrolSpeed * Time.deltaTime * direction);
		}
		base.Update();
	}

	public override void Exit()
	{
		base.Exit();
	}

	IEnumerator Stalling()
	{
		float stallTime = Random.Range(stallTimeMin, stallTimeMax);
		yield return new WaitForSeconds(stallTime);
		stalling = false;
	}

	IEnumerator WaitForPathToBeCalculated()
	{
		while (!enemyInfo.rm.calculatedPatrols)
		{
			yield return null;
		}
		path = enemyInfo.rm.GetPatrol(enemyInfo.enemyId);
		stalling = false;
	}
}
