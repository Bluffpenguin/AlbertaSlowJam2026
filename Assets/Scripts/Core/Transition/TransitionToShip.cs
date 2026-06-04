using UnityEngine;

public class TransitionToShip : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip;
	[SerializeField] private Vector2 _tooltipOffset;

	[SerializeField] private float _interactTime = 1f;
	public float InteractTime => _interactTime;

	[Header("Outline (Optional)")]
	[SerializeField] private Renderer _renderer;
	[SerializeField] private Material _regularMaterial;
	[SerializeField] private Material _outlineMaterial;

	public string Tooltip => _tooltip;

	public bool CanInteract => enabled;

	public Vector2 TooltipOffset => _tooltipOffset;

	public Renderer MaterialRenderer => _renderer;
	public Material RegularMaterial => _regularMaterial;

	public Material OutlineMaterial => _outlineMaterial;

	

	public void OnInteract()
	{
		if (TransitionManager.Instance != null) Debug.Log("There is a singleton");
		TransitionManager.Instance.TransitionToShip.Invoke();
	}

	public void ToggleOutline(bool outline)
	{
		if (_renderer == null) return;


		if (outline)
		{
			_renderer.material = _outlineMaterial;
		}
		else
			_renderer.material = _regularMaterial;
	}
}
