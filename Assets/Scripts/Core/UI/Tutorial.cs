using TMPro;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _tutorialText;
	[SerializeField] private TextMeshProUGUI _buttonLabel;
	[SerializeField][TextArea] private string[] _messages = new string[1];
	[SerializeField][Min(0)] private int _index = 0;
	[Space]
	[SerializeField] private UnityEvent _onEnable = new();
	[SerializeField] private UnityEvent _onDisable = new();

	protected virtual void OnEnable()
	{
		_onEnable.Invoke();
		if (Player.Exists) {
			Player.Input.Player.Disable();
		}
		MoveNext();
	}

	protected virtual void OnDisable()
	{
		_onDisable.Invoke();
		if (Player.Exists) {
			Player.Input.Player.Enable();
		}
	}

	public void MoveNext()
	{
		if (_index < 0 || _index >= _messages.Length) {
			Close();
			return;
		}

		if (_tutorialText != null) {
			_tutorialText.text = _messages[_index];
		}

		_index++;

		if (_buttonLabel != null) {
			_buttonLabel.text = _index >= _messages.Length ? "Close" : "Next";
		}
	}

	public void Close()
	{
		Destroy(this.gameObject);
	}
}
