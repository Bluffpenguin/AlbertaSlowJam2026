using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
	[field: Header("SFX")]
	[field: SerializeField] public EventReference PickUpItem { get; private set; }
	[field: SerializeField] public EventReference MoneyGained { get; private set; }

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
