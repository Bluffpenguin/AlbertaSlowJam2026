using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] Transform _tooltipTransform;
    [SerializeField] TextMeshProUGUI _tooltipTextObj;
    [SerializeField] string _loadedTooltip;

    public static UnityEvent<Vector3, string> CallTooltip = new UnityEvent<Vector3, string>();
    public static UnityEvent DismisTooltip = new UnityEvent();

	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Awake()
	{
		CallTooltip.AddListener(OpenTooltip);
		DismisTooltip.AddListener(CloseTooltip);
		_tooltipTextObj.gameObject.SetActive(false);
	}

	void OpenTooltip(Vector3 pos, string tip)
	{
		_loadedTooltip = tip;
		_tooltipTransform.position = pos;
		_tooltipTextObj.text = "[E] - " + tip;
		_tooltipTextObj.gameObject.SetActive(true);
	}

	void CloseTooltip()
	{
		_tooltipTextObj.gameObject.SetActive(false);
		_loadedTooltip = null;
		_tooltipTextObj.text = "";
	}
}
