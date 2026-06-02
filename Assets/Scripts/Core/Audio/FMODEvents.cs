using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
	[field:Header("Music")]
	[field: SerializeField] public EventReference music { get; private set; }
	public static FMODEvents Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(Instance);
		}
	}
}
