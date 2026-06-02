using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
	private List<EventInstance> eventInstances;
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
