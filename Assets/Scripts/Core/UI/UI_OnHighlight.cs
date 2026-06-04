using UnityEngine;
using UnityEngine.UI;

public class UI_OnHighlight : MonoBehaviour
{
    Button button;
    [SerializeField] RectTransform _leftGear, _rightGear;
    [SerializeField] float _leftGearSpeed, _rightGearSpeed;
    bool isHighlighted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHighlighted)
        {
			_leftGear.Rotate(new Vector3(0, 0, _leftGearSpeed * Time.deltaTime));
			_rightGear.Rotate(new Vector3(0, 0, _rightGearSpeed * Time.deltaTime));
		}
    }

    public void Highlight(bool isPointerOverButton)
    {
        isHighlighted = isPointerOverButton;
    }

    
}
