using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
	Animator anim;
	public Transform player;
	AIState currentState;
	internal RoomManager rm;
	[SerializeField] Transform heading;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		//anim = this.GetComponent<Animator>();
		player = GameObject.FindWithTag("Player").transform;
		currentState = new State_SimpleIdle(this.gameObject, anim, player, rm, heading);
	}

	// Update is called once per frame
	void Update()
	{
		currentState = currentState.Process();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;

		var direction = Quaternion.AngleAxis(currentState.visAngle, heading.forward) * heading.up;
		Gizmos.DrawRay(transform.position, direction * (3 / rm.tileMap.cellSize.magnitude));
		

		direction = Quaternion.AngleAxis(-currentState.visAngle, heading.forward) * heading.up;
		Gizmos.DrawRay(transform.position, direction * (3 / rm.tileMap.cellSize.magnitude));


	}
}
