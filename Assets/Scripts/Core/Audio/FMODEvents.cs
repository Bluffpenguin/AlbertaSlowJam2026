using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
	[field: Header("SFX")]

	[field: Header("Music")]
	[field: SerializeField] public EventReference ShopMusic { get; private set; }
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
