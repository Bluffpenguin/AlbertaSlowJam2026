using UnityEngine.Events;

public class Hatch : MonoBehaviour
{
	public UnityEvent OnEnter, OnExit;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{
		TransitionManager.Instance.MidDungeonTransition.AddListener(OnExit.Invoke);
		TransitionManager.Instance.MidShipTransition.AddListener(OnEnter.Invoke);
	}
}
