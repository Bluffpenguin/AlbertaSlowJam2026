using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
	[field: Header("Volume")]
	[Range(0, 1)]
	public float masterVolume = 1;

	[Range(0, 1)]
	public float musicVolume = 1;

	[Range(0, 1)]
	public float sfxVolume = 1;

	private Bus masterBus;
	private Bus musicBus;
	private Bus sfxBus;

	private List<EventInstance> eventInstances;

	private EventInstance musicEventInstance;
	public static AudioManager Instance { get; private set; }

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
		eventInstances = new List<EventInstance>();

		masterBus = RuntimeManager.GetBus("bus:/");
		musicBus = RuntimeManager.GetBus("bus:/Music");
		sfxBus = RuntimeManager.GetBus("bus:/SFX");
	}

	private void Update()
	{
		masterBus.setVolume(masterVolume);
		musicBus.setVolume(musicVolume);
		sfxBus.setVolume(sfxVolume);
	}

	public void InitializeMusic(EventReference musicEventReference)
	{
		musicEventInstance = CreateEventInstance(musicEventReference);
		musicEventInstance.start();
	}

	public void PlayOneShot(EventReference sound, Vector3 worldPos)
	{
		RuntimeManager.PlayOneShot(sound, worldPos);
	}

	public EventInstance CreateEventInstance(EventReference eventReference)
	{
		EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
		eventInstances.Add(eventInstance);
		return eventInstance;
	}

	private void CleanUp()
	{
		foreach (EventInstance eventInstance in eventInstances)
		{
			eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			eventInstance.release();
		}
	}

	private void OnDestroy()
	{
		CleanUp();
	}
}
