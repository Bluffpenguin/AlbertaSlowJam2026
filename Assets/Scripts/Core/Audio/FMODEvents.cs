using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
	[field: Header("SFX")]

	// Interactables
	[field: SerializeField] public EventReference PickUpItem { get; private set; }
	[field: SerializeField] public EventReference MoneyGained { get; private set; }
	[field: SerializeField] public EventReference OpenStorage { get; private set; }
	[field: SerializeField] public EventReference CloseStorage { get; private set; }
	[field: SerializeField] public EventReference EnterBed { get; private set; }
	[field: SerializeField] public EventReference OpenShipDoor { get; private set; }

	// Player
	[field: SerializeField] public EventReference PlayerFootsteps { get; private set; }
	[field: SerializeField] public EventReference PlayerDash { get; private set; }
	[field: SerializeField] public EventReference PlayerStunned { get; private set; }

	// UI
	[field: SerializeField] public EventReference ButtonHover { get; private set; }
	[field: SerializeField] public EventReference ButtonHiss { get; private set; }
	[field: SerializeField] public EventReference PlayGameButtonWhistle { get; private set; }

	// Enemy
	[field: SerializeField] public EventReference EnemyStep { get; private set; }

	[field: Header("Music")]
	[field: SerializeField] public EventReference GameMusic { get; private set; }

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
