public class Ladder : Hatch
{
	protected override void Start()
	{
		TransitionManager.Instance.MidDungeonTransition.AddListener(OnEnter.Invoke);
		TransitionManager.Instance.MidShipTransition.AddListener(OnExit.Invoke);
	}
}
