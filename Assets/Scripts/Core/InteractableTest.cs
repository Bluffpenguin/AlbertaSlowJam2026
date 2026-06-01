using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
	[SerializeField] private string _tooltip;

    public string Tooltip => _tooltip;

	public bool CanInteract => enabled;

	public Vector2 TooltipOffset => throw new NotImplementedException();

	public Renderer MaterialRenderer => throw new NotImplementedException();

	public Material RegularMaterial => throw new NotImplementedException();

	public Material OutlineMaterial => throw new NotImplementedException();

	[SerializeField] private float _interactTime = 1f;
	public float InteractTime => _interactTime;

	public void OnInteract()
	{
		Debug.Log(Tooltip);
	}

	public void ToggleOutline(bool outline)
	{
		throw new NotImplementedException();
	}
}
