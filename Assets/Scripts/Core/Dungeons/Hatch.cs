using UnityEngine;
using UnityEngine.Events;

public class Hatch : MonoBehaviour
{
	public UnityEvent OnEnter, OnExit;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{
		TransitionManager.Instance.TransitionToDungeon.AddListener(OnExit.Invoke);
		TransitionManager.Instance.TransitionToShip.AddListener(OnEnter.Invoke);
	}
}
