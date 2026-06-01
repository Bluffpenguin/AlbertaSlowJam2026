using UnityEngine;

public class TransitionToDungeon : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip;
	[SerializeField] private Vector2 _tooltipOffset;

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
		TransitionManager.Instance.TransitionToDungeon.Invoke();
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
