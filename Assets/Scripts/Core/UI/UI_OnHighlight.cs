using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;

public class UI_OnHighlight : MonoBehaviour
{
    Button button;
    [SerializeField] RectTransform _leftGear, _rightGear;
    [SerializeField] float _leftGearSpeed, _rightGearSpeed;
    bool isHighlighted = false;
	private EventInstance _buttonHover;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        button = GetComponent<Button>();
        //_buttonHover = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.ButtonHover);
	}

    // Update is called once per frame
    void Update()
    {
        if (isHighlighted)
        {
			_leftGear.Rotate(new Vector3(0, 0, _leftGearSpeed * Time.deltaTime));
			_rightGear.Rotate(new Vector3(0, 0, _rightGearSpeed * Time.deltaTime));
		}
		//UpdateSound();
	}

    public void Highlight(bool isPointerOverButton)
    {
        isHighlighted = isPointerOverButton;
    }

	private void UpdateSound()
	{
		// start footsteps event if the player is moving
		if (isHighlighted)
		{
			// get playback state
			PLAYBACK_STATE playbackState;
			_buttonHover.getPlaybackState(out playbackState);

			if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
			{
				_buttonHover.start();
			}
		}
		// otherwise stop the footsteps event
		else
		{
			_buttonHover.stop(STOP_MODE.ALLOWFADEOUT);
		}
	}
}
