using UnityEngine;

public class WP_Gizmo : MonoBehaviour
{
	readonly float size = 0.1f;
	[SerializeField] private bool gizmoActive = true;

	private void OnDrawGizmos()
	{
		if (!gizmoActive) return;
		Gizmos.color = Color.orange;
		Gizmos.DrawSphere(transform.position, size);
		Gizmos.color = Color.white;
	}
}
