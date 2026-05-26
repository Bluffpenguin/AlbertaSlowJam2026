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
		var direction = Quaternion.AngleAxis(currentState.visAngle, heading.forward) * (currentState.visDist * rm.tileMap.cellSize.magnitude * heading.up);
		Gizmos.DrawRay(transform.position, direction);

		direction = Quaternion.AngleAxis(-currentState.visAngle, heading.forward) * (currentState.visDist * rm.tileMap.cellSize.magnitude * heading.up);
		Gizmos.DrawRay(transform.position, direction);
	}
}
