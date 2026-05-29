using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
	[SerializeField] GameObject playerPrefab;

	private void Awake()
	{
		Instantiate(playerPrefab, transform.position, Quaternion.identity);
	}
}
