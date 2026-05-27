using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class State_SimplePatrol : AIState
{
	float patrolSpeed = 0.5f;
	List<Vector2Int> patrolWPs = new List<Vector2Int>();
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
		patrolWPs = enemyInfo.rm.GetPatrol(enemyInfo.enemyId);
		if (patrolWPs.Count > 0 )
		{
			Move_Shaved(patrolWPs[0], patrolWPs[1]);
			enemyInfo.npc.transform.position = path[0].getId().transform.position;
		}
		
        base.Enter();
	}

	public override void Update()
	{
		//DEBUG
		if ( Keyboard.current.kKey.wasPressedThisFrame)
		{
			patrolWPs = enemyInfo.rm.GetPatrol(enemyInfo.enemyId);
		}
		//


		if (path.Count == 0)
		{
			nextState = new State_SimpleIdle(enemyInfo, player);
			return;
		}
			
		if (CanSeePlayer())
		{
			//nextState = new State_SimplePursue(enemyInfo, player);
			return;
		}

		if (stalling)
			return;

		if (currentWP == path.Count)
		{
			/*
			if (currentPatrolIndex + 1 == patrolWPs.Count)
			{
				reversePatrol = true;
			}
			else if (currentPatrolIndex == 0 && reversePatrol)
			{
				reversePatrol = false;
			}

			if (reversePatrol)
			{
				currentWP = 0;
				Move_Shaved(patrolWPs[currentPatrolIndex], patrolWPs[currentPatrolIndex-1]);
				currentPatrolIndex--;

			}
			else
			{
				currentWP = 0;
				Move_Shaved(patrolWPs[currentPatrolIndex], patrolWPs[currentPatrolIndex + 1]);
				currentPatrolIndex++;
			}

			stalling = true;
			enemyInfo.rm.StartCoroutine(Stalling());
			*/

			currentWP = 0;
			enemyInfo.npc.transform.position = path[0].getId().transform.position;

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

			Vector2 direction = lookAtGoal - (Vector2)enemyInfo.npc.transform.position;

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
}
