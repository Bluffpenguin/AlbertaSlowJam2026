using UnityEngine;

public class Anim_TitleCard : MonoBehaviour
{
	[Header("Title Animation")]
	[SerializeField] bool _useAnim;
	[Space]
	[SerializeField] Transform _gear1;
	[SerializeField] int _rotationSpeed1 = 10;
	[Space]
	[SerializeField] Transform _gear2;
	[SerializeField] int _rotationSpeed2 = 10;
	[Space]
	[SerializeField] Transform _gear3;
	[SerializeField] int _rotationSpeed3 = 10;
	[Space]
	[SerializeField] Transform _gear4;
	[SerializeField] int _rotationSpeed4 = 10;
	[Space]
	[SerializeField] Transform _gear5;
	[SerializeField] int _rotationSpeed5 = 10;
	[Space]
	[SerializeField] Transform _gear6;
	[SerializeField] int _rotationSpeed6 = 10;
	[Space]
	[SerializeField] Transform _gear7;
	[SerializeField] int _rotationSpeed7 = 10;
	[Space]
	[SerializeField] Transform _gear8;
	[SerializeField] int _rotationSpeed8 = 10;

	private void FixedUpdate()
	{
		if (!_useAnim) return;

		_gear1.Rotate(new Vector3(0, 0, _rotationSpeed1 * Time.fixedDeltaTime));
		_gear2.Rotate(new Vector3(0, 0, _rotationSpeed2 * Time.fixedDeltaTime));
		_gear3.Rotate(new Vector3(0, 0, _rotationSpeed3 * Time.fixedDeltaTime));
		_gear4.Rotate(new Vector3(0, 0, _rotationSpeed4 * Time.fixedDeltaTime));
		_gear5.Rotate(new Vector3(0, 0, _rotationSpeed5 * Time.fixedDeltaTime));
		_gear6.Rotate(new Vector3(0, 0, _rotationSpeed6 * Time.fixedDeltaTime));
		_gear7.Rotate(new Vector3(0, 0, _rotationSpeed7 * Time.fixedDeltaTime));
		_gear8.Rotate(new Vector3(0, 0, _rotationSpeed8 * Time.fixedDeltaTime));

	}
}
