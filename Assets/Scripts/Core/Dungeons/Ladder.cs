public class Ladder : Hatch
{
	protected override void Start()
	{
		TransitionManager.Instance.TransitionToDungeon.AddListener(OnEnter.Invoke);
		TransitionManager.Instance.TransitionToShip.AddListener(OnExit.Invoke);
	}
}
