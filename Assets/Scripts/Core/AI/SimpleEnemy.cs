using UnityEngine;

[System.Serializable]
public struct EnemyInfo { 
	internal Animator anim;
	public int enemyId;
	public Transform heading;
	public GameObject npc;
	public RoomManager rm;
	public LayerMask sightObstructions;
	public AIState.STATE defaultState;
	public Rigidbody2D rb;
	public float attackRange;
}

public class SimpleEnemy : MonoBehaviour
{
	
	Transform player;
	AIState currentState;
	public EnemyInfo enemyInfo = new EnemyInfo { };
	[SerializeField] bool active = false;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		//enemyInfo.anim = this.GetComponent<Animator>();
		enemyInfo.attackRange = 2;
		enemyInfo.npc = this.gameObject;
		enemyInfo.rb = GetComponent<Rigidbody2D>();
		player = GameObject.FindWithTag("Player").transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (!active) return;

		if (enemyInfo.rm == null) Destroy(this.gameObject);
		currentState = currentState.Process();
		
	}

	private void FixedUpdate()
	{
		if (!active) return;

		currentState.ProcessFixed();
	}

	public void SetActive(bool isActive)
	{
		if (enemyInfo.defaultState == AIState.STATE.IDLE)
			currentState = new State_SimpleIdle(enemyInfo, player);
		else
			currentState = new State_SimplePatrol(enemyInfo, player);

		active = isActive;
	}

	public void Remove()
	{
		gameObject.SetActive(false);
	}

	private void OnDrawGizmosSelected()
	{
		if (currentState == null)
			return;
		Gizmos.color = Color.white;

		var direction = Quaternion.AngleAxis(currentState.visAngle, enemyInfo.heading.forward) * enemyInfo.heading.up;
		Gizmos.DrawRay(transform.position, direction * (3 / enemyInfo.rm.tileMap.cellSize.magnitude));
		

		direction = Quaternion.AngleAxis(-currentState.visAngle, enemyInfo.heading.forward) * enemyInfo.heading.up;
		Gizmos.DrawRay(transform.position, direction * (3 / enemyInfo.rm.tileMap.cellSize.magnitude));


	}
}
