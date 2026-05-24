using UnityEngine;

public class WP_Gizmo : MonoBehaviour
{
	readonly float size = 0.1f;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.orange;
		Gizmos.DrawSphere(transform.position, size);
		Gizmos.color = Color.white;
	}
}
