using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class AIState
{
	public enum STATE
	{
		IDLE, PATROL, PURSUE
	};

	public enum EVENT
	{
		ENTER, UPDATE, EXIT
	};

	public STATE name;
	protected EVENT stage;
	protected GameObject npc;
	protected Animator anim;
	protected Transform player;
	protected AIState nextState;
	protected RoomManager rm;
	protected Transform heading;

	internal float visDist = 3.0f;
	internal float visAngle = 30.0f;


	public AIState(GameObject _npc, Animator _anim, Transform _player, RoomManager _rm, Transform _heading)
	{
		npc = _npc;
		anim = _anim;
		stage = EVENT.ENTER;
		player = _player;
		rm = _rm;
		heading = _heading;
	}

	public virtual void Enter() { stage = EVENT.UPDATE; }
	public virtual void Update() { stage = EVENT.UPDATE; }
	public virtual void Exit() { stage = EVENT.EXIT; }

	public AIState Process()
	{
		if (stage == EVENT.ENTER) Enter();
		if (stage == EVENT.UPDATE) Update();
		if (stage == EVENT.EXIT)
		{
			Exit();
			return nextState;
		}
		return this;
	}

	public bool CanSeePlayer()
	{
		
		Vector3 direction = player.position - npc.transform.position;
		float angle = Vector3.Angle(direction, heading.up);
		float scaledDist = direction.magnitude * rm.tileMap.cellSize.magnitude;

		if (scaledDist  < visDist && angle < visAngle)
		{
			return true;
		}
		return false;
	}

	
}
