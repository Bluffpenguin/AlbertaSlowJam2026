public class DungeonDisabler : MonoBehaviour
{
	[SerializeField]
	[Min(0)] private float _timeDelay = 1;
	[SerializeField] private GameObject _grid;

	public void StartRoutine()
	{
		StartCoroutine(DisableAfter());
	}

	public IEnumerator DisableAfter()
	{
		yield return new WaitForSeconds(_timeDelay);

		if (_grid != null)
			_grid.SetActive(false);
	}
}
