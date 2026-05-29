using UnityEngine.Events;

[Obsolete]
public class DungeonTransition : MonoBehaviour
{
	[SerializeField] private Vector3 _workshopPosition = new(0, 0, -10);
	[SerializeField] private UnityEvent _onEnterDungeon, _onExitDungeon;
	[SerializeField] private UnityEvent _onEnterWorkshop, _onExitWorkshop;

	public void MoveToDungeon()
	{
		_onExitWorkshop.Invoke();
		Player.Controller.transform.position = Vector3.zero;
		_onEnterDungeon.Invoke();
	}

	public void MoveFromDungeon()
	{
		_onExitDungeon.Invoke();
		Player.Controller.transform.position = _workshopPosition;
		_onEnterWorkshop.Invoke();
	}
}
